using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading;
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
        internal StringBuilder? Error { get; set; }
        internal int InputBuffer { get; set; } = 4096;
        internal int OutputBuffer { get; set; } = 4096;
        internal CancellationToken CancellationToken { get; set; }

        private readonly List<Task> _tasks = new();

        internal FFProcess()
        {
            //Don't allow end user to create process directly
        }

        public Task<FFProcess> StartAsync() => Task.Run(async () =>
        {
            await StartProcess()._tasks.WhenAll();
            return this;
        }, CancellationToken);

        public async Task<T?> DeserializeResultAsync<T>(string property)
        {
            await _tasks.WhenAll();

            if (Output is null || ExitCode is not 0 || CancellationToken.IsCancellationRequested)
                return default;

            if (Output.CanSeek)
                Output.Seek(0, SeekOrigin.Begin);
            else
                throw new InvalidOperationException("Output stream type cannot seek");

            try
            {
                var document = await JsonDocument.ParseAsync(Output, cancellationToken: CancellationToken);
                document.RootElement.TryGetProperty(property, out JsonElement result);
                return result.ToObject<T>();
            }
            catch
            {
                return default;
            }
        }

        private async Task PipeInput()
        {
            if (Input is null)
            {
                Kill();
                throw new NullReferenceException("Input cannot be null");
            }
            var bytes = new byte[InputBuffer];
            int bytesRead;

            while (!CancellationToken.IsCancellationRequested && !HasExited && (bytesRead = await Input.ReadAsync(bytes, CancellationToken).NotThrow()) is not 0)
                await StandardInput.BaseStream.WriteAsync(bytes, 0, bytesRead, CancellationToken).NotThrow();

            StandardInput.Close();
        }

        private async Task PipeOutput()
        {
            var bytes = new byte[OutputBuffer];
            int bytesRead;

            while (!CancellationToken.IsCancellationRequested && (bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes, CancellationToken).NotThrow()) is not 0)
            {
                if (Output is not null)
                    await Output.WriteAsync(bytes, 0, bytesRead, CancellationToken).NotThrow();

                CallOutputEvent(bytes.Take(bytesRead).ToArray());
            }
        }

        private async Task PipeError()
        {
            while (!CancellationToken.IsCancellationRequested && !StandardError.EndOfStream)
            {
                var line = await StandardError.ReadLineAsync();
                CallErrorEvent(line);
                Error?.AppendLine(line);

            }
        }

        private FFProcess StartProcess()
        {
            Exited += new EventHandler(CallExitEvent);
            Start();

            if (StartInfo.RedirectStandardOutput)
                _tasks.Add(PipeOutput());

            if (StartInfo.RedirectStandardInput)
                _tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardError)
                _tasks.Add(PipeError());

            _tasks.Add(Task.Run(WaitForExit, CancellationToken).NotThrow());

            Task.Run(KillProcess, CancellationToken);

            return this;
        }

        /// <summary>
        /// Kill the process if cancellation is requested beacuse ffmpeg may be waiting for pipe data
        /// </summary>
        internal async Task KillProcess()
        {
            await Task.WhenAll(_tasks);
            if (CancellationToken.IsCancellationRequested)
                Kill();
        }

        private void CallOutputEvent(byte[] bytes) => OutputDataReceived?.Invoke(this, bytes);
        private void CallErrorEvent(string? messageException) => ErrorDataReceived?.Invoke(this, messageException);
        private void CallExitEvent(object? sender, EventArgs e)
        {
            var process = (Process?)sender;

            if (process?.ExitCode is not 0)
                ExitedWithError?.Invoke(this);
        }
    }
}
