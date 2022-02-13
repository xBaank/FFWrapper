using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Extensions;

namespace FFmpegWrapper.Models
{
    /// <summary>
    /// FFprobeprocess, use FFprobeProcessBuilder to create a FFprobeprocess or FFprobeClient to get metadata
    /// </summary>
    public class FFProbeProcess : Process
    {


        public new event Action<FFProbeProcess, string?>? ErrorDataReceived;
        public event Action<FFProbeProcess>? ExitedWithError;

        internal Stream? Input { get; set; }
        internal string? Error { get; set; }
        internal string? Output { get; set; }
        internal int InputBuffer { get; set; } = 4096;

        private List<Task> tasks = new List<Task>();


        internal FFProbeProcess()
        {
            //Don't allow end user to create process directly
        }

        public new void Start() => StartProcess().tasks.WaitAll();
        public Task StartAsync() => StartProcess().tasks.WhenAll();

        private Task PipeInput()
        {
            if (Input == null)
                throw new NullReferenceException("Input set to null");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[InputBuffer];
                int bytesRead;

                while ((bytesRead = await Input.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await StandardInput.BaseStream.WriteAsync(bytes, 0, bytesRead);

                StandardInput.Close();
            });
        }

        private Task PipeOutput()
        {

            return Task.Run(async () =>
            {
                while (!StandardOutput.EndOfStream)
                {
                    var line = await StandardOutput.ReadLineAsync();
                    Output += line;
                }

            });
        }

        private Task PipeError()
        {
            return Task.Run(async () =>
            {
                while (!StandardError.EndOfStream)
                {
                    var line = await StandardError.ReadLineAsync();
                    CallErrorEvent(line);
                    Error += line;
                }
            });
        }

        private FFProbeProcess StartProcess()
        {
            Exited += new EventHandler(CallExitEvent);
            base.Start();

            if (StartInfo.RedirectStandardInput)
                tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardOutput)
                tasks.Add(PipeOutput());

            if (StartInfo.RedirectStandardError)
                tasks.Add(PipeError());


            return this;
        }

        private void CallErrorEvent(string? messageException) => ErrorDataReceived?.Invoke(this, messageException);
        private void CallExitEvent(object? sender, EventArgs e)
        {
            Process? process = (Process?)sender;

            if (process?.ExitCode != 0)
                ExitedWithError?.Invoke(this);
        }
    }
}
