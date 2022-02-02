using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Extensions;

namespace FFmpegWrapper.Models
{
    public class FFmpegProcess : Process
    {

        private List<Task> tasks = new List<Task>();

        public Stream? Input { get; set; }
        public Stream? Output { get; set; }
        public bool isOutputEventRaised { get; set; } = false;
        public bool isErrorEventRaised { get; set; } = false;

        public void Start(Stream? input = default, Stream? output = default) => StartProcess().tasks.WaitAll();
        public Task StartAsync(Stream? input = default, Stream? output = default) => StartProcess().tasks.WhenAll();

        private Task PipeInput()
        {
            if (Input == null)
                throw new Exception("Input set to null");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[4096];
                int bytesRead;

                while ((bytesRead = await Input.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await StandardInput.BaseStream.WriteAsync(bytes, 0, bytesRead);

                StandardInput.Close();
            });
        }

        private Task PipeOutput()
        {
            if (Output == null)
                throw new Exception("Output set to null");

            if (isOutputEventRaised)
                throw new InvalidOperationException("Cannot convert to stream with outputEvent raised");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[4096];
                int bytesRead;

                while ((bytesRead = await StandardOutput.BaseStream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await Output.WriteAsync(bytes, 0, bytesRead);

            });
        }

        private FFmpegProcess StartProcess()
        {
            base.Start();

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
