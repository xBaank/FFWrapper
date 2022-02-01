using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

using WebmOpus.Extensions;

namespace WebmOpus.Models
{
    public class FFmpegProcess : Process
    {
        public Stream? Input { get; set; }
        public Stream? Output { get; set; }
        public bool isOutputEventRaised { get; set; } = false;
        public bool isErrorEventRaised { get; set; } = false;

        public FFmpegProcess ToStream(Stream output, MediaTypes type)
        {
            StartInfo.RedirectStandardOutput = true;
            StartInfo.Arguments += $" -f {type.GetArgs()} pipe:";
            Output = output;

            return this;
        }

        public FFmpegProcess To(string output)
        {
            StartInfo.RedirectStandardOutput = false;
            StartInfo.Arguments += $" {output}";

            return this;
        }

        public FFmpegProcess From(Stream input, MediaTypes type)
        {
            StartInfo.Arguments = $"-f {type.GetArgs()} -i pipe:";
            StartInfo.RedirectStandardInput = true;
            Input = input;

            return this;
        }

        public FFmpegProcess From(string input)
        {
            StartInfo.Arguments = $"-i {input}";
            StartInfo.RedirectStandardInput = true;

            return this;
        }

        public FFmpegProcess RaiseOutputEvents(Action<object, DataReceivedEventArgs> action)
        {
            StartInfo.RedirectStandardOutput = true;
            isOutputEventRaised = true;
            OutputDataReceived += new DataReceivedEventHandler(action);

            return this;
        }

        public FFmpegProcess RaiseErrorEvents(Action<object, DataReceivedEventArgs> action)
        {
            StartInfo.RedirectStandardError = true;
            isErrorEventRaised = true;
            ErrorDataReceived += new DataReceivedEventHandler(action);

            return this;
        }

        public FFmpegProcess StartProcess(Stream? input = default, Stream? output = default)
        {
            Start();

            if (StartInfo.RedirectStandardError && isErrorEventRaised)
                BeginErrorReadLine();

            if (StartInfo.RedirectStandardOutput && isOutputEventRaised)
                BeginOutputReadLine();


            List<Task> tasks = new List<Task>();

            if (StartInfo.RedirectStandardInput)
                tasks.Add(PipeInput());

            if (StartInfo.RedirectStandardOutput)
                tasks.Add(PipeOutput());

            Task.WaitAll(tasks.ToArray());

            return this;
        }

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
    }
}
