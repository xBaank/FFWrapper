using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using FFmpegWrapper.Extensions;

namespace FFmpegWrapper.Models
{
    /// <summary>
    /// FFmpeg process, use FFmpegProcessBuilder to create a FFmpegProcess or FFmpegClient to convert mediaFiles
    /// </summary>
    public class FFProcess : Process
    {


        public new event Action<FFProcess, string?>? ErrorDataReceived;
        public new event Action<FFProcess, byte[]>? OutputDataReceived;
        public event Action<FFProcess>? ExitedWithError;

        internal Stream? Input { get; set; }
        internal Stream? Output { get; set; }
        internal string? Error { get; set; }
        internal int InputBuffer { get; set; } = 4096;
        internal int OutputBuffer { get; set; } = 4096;

        private List<Task> tasks = new List<Task>();


        internal FFProcess()
        {
            //Don't allow end user to create process directly
        }

        public Task StartAsync() => StartProcess().tasks.WhenAll();

        public async Task<T?> DeserializeResultAsync<T>()
        {
            WaitForExit();

            if (Output is null || ExitCode != 0)
                return default;

            Output.Seek(0, SeekOrigin.Begin);

            var result = await JsonSerializer.DeserializeAsync<T?>(Output);
            return result;
        }

        private async Task PipeInput()
        {
            if (Input is null)
            {
                Kill();
                throw new NullReferenceException("Input cannot be null");
            }
            byte[] bytes = new byte[InputBuffer];
            int bytesRead;

            while (!HasExited && (bytesRead = await Input.ReadAsync(bytes)) != 0)
                await StandardInput.BaseStream.WriteAsync(bytes.AsMemory(0, bytesRead));

            StandardInput.Close();
        }

        private async Task PipeOutput()
        {
            byte[] bytes = new byte[OutputBuffer];
            int bytesRead;

            while (!HasExited && (bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes)) != 0)
            {
                if (Output is not null)
                    await Output.WriteAsync(bytes, 0, bytesRead);

                CallOutputEvent(bytes.Take(bytesRead).ToArray());
            }
        }

        private async Task PipeError()
        {
            while (!StandardError.EndOfStream)
            {
                var line = await StandardError.ReadLineAsync();
                CallErrorEvent(line);
                Error += line;

            }
        }

        private FFProcess StartProcess()
        {
            Exited += new EventHandler(CallExitEvent);
            Start();

            if (StartInfo.RedirectStandardOutput)
                tasks.Add(PipeOutput());

            if (StartInfo.RedirectStandardInput)
                tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardError)
                tasks.Add(PipeError());


            return this;
        }

        private void CallOutputEvent(byte[] bytes) => OutputDataReceived?.Invoke(this, bytes);
        private void CallErrorEvent(string? messageException) => ErrorDataReceived?.Invoke(this, messageException);
        private void CallExitEvent(object? sender, EventArgs e)
        {
            Process? process = (Process?)sender;

            if (process?.ExitCode != 0)
                ExitedWithError?.Invoke(this);
        }
    }
}
