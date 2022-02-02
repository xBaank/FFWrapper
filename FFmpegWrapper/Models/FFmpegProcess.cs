using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

using FFmpegWrapper.Extensions;

namespace FFmpegWrapper.Models
{
    /// <summary>
    /// FFmpeg process, use FFmpegProcessBuilder to create a FFmpegProcess or FFmpegClient to convert mediaFiles
    /// </summary>
    public class FFmpegProcess : Process
    {


        public new event Action<object, string?>? ErrorDataReceived;
        public new event Action<object, byte[]>? OutputDataReceived;

        internal Stream? Input { get; set; }
        internal Stream? Output { get; set; }
        internal string? Error { get; set; }
        internal int InputBuffer { get; set; } = 4096;
        internal int OutputBuffer { get; set; } = 4096;

        private List<Task> tasks = new List<Task>();


        internal FFmpegProcess()
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
            if (Output == null)
                throw new NullReferenceException("Output set to null");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[OutputBuffer];
                int bytesRead;

                while ((bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                {
                    await Output.WriteAsync(bytes, 0, bytesRead);
                    CallOutputEvent(bytes);
                }

            });
        }

        private Task PipeError()
        {
            return Task.Run(async () =>
            {
                StringBuilder stringBuilder = new StringBuilder();

                while (!StandardError.EndOfStream)
                {
                    string? line = await StandardError.ReadLineAsync();
                    CallErrorEvent(line);
                    stringBuilder.AppendLine(line);
                }

                CallErrorEvent(stringBuilder.ToString());

            });
        }

        private FFmpegProcess StartProcess()
        {
            base.Start();

            if (StartInfo.RedirectStandardInput)
                tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardOutput)
                tasks.Add(PipeOutput());

            if (StartInfo.RedirectStandardError)
                tasks.Add(PipeError());

            return this;
        }

        private void CallOutputEvent(byte[] bytes) => OutputDataReceived?.Invoke(this, bytes);
        private void CallErrorEvent(string? messageException) => ErrorDataReceived?.Invoke(this, messageException);
    }
}
