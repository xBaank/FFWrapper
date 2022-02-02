using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Extensions;

namespace FFmpegWrapper.Models
{
    /// <summary>
    /// FFmpeg process, use FFmpegProcessBuilder to create a FFmpegProcess or FFmpegClient to convert mediaFiles
    /// </summary>
    public class FFmpegProcess : Process
    {

        /// <inheritdoc />
        public new event Func<Exception>? ErrorDataReceived;
        /// <inheritdoc />
        public new event Func<byte[]>? OutputDataReceived;

        private List<Task> tasks = new List<Task>();

        internal Stream? Input { get; set; }
        internal Stream? Output { get; set; }
        internal bool isOutputEventRaised { get; set; } = false;
        internal bool isErrorEventRaised { get; set; } = false;
        internal int InputBuffer { get; set; } = 4096;
        internal int OutputBuffer { get; set; } = 4096;

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

            if (isOutputEventRaised)
                throw new InvalidOperationException("Cannot convert to stream with outputEvent raised");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[OutputBuffer];
                int bytesRead;

                while ((bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await Output.WriteAsync(bytes, 0, bytesRead);

            });
        }

        private FFmpegProcess StartProcess()
        {
            base.Start();

            //TODO don't use default process events
            if (StartInfo.RedirectStandardError && isErrorEventRaised)
                BeginErrorReadLine();

            if (StartInfo.RedirectStandardOutput && isOutputEventRaised)
                BeginOutputReadLine();


            if (StartInfo.RedirectStandardInput)
                tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardOutput)
                tasks.Add(PipeOutput());

            return this;
        }
    }
}
