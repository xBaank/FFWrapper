using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

using WebmOpus.Models;

namespace WebmOpus.Extensions
{
    public static class FFmpegProcessExtensions
    {

        internal static FFmpegProcess ToStream(this FFmpegProcess process, Stream output, MediaTypes type)
        {
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments += $" -f {type.GetArgs()} pipe:";
            process.Output = output;

            return process;
        }

        internal static FFmpegProcess To(this FFmpegProcess process, string output)
        {
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.Arguments += $" {output}";

            return process;
        }

        internal static FFmpegProcess From(this FFmpegProcess process, Stream input, MediaTypes type)
        {
            process.StartInfo.Arguments = $"-f {type.GetArgs()} -i pipe:";
            process.StartInfo.RedirectStandardInput = true;
            process.Input = input;

            return process;
        }

        internal static FFmpegProcess From(this FFmpegProcess process, string input)
        {
            process.StartInfo.Arguments = $"-i {input}";
            process.StartInfo.RedirectStandardInput = true;
            return process;
        }

        internal static FFmpegProcess RaiseOutputEvents(this FFmpegProcess process, Action<Object, DataReceivedEventArgs> action)
        {
            process.StartInfo.RedirectStandardOutput = true;
            process.isOutputEventRaised = true;
            process.OutputDataReceived += new DataReceivedEventHandler(action);
            return process;
        }

        internal static FFmpegProcess RaiseErrorEvents(this FFmpegProcess process, Action<Object, DataReceivedEventArgs> action)
        {
            process.StartInfo.RedirectStandardError = true;
            process.isErrorEventRaised = true;
            process.ErrorDataReceived += new DataReceivedEventHandler(action);
            return process;
        }

        internal static FFmpegProcess StartProcess(this FFmpegProcess process, Stream? input = default, Stream? output = default)
        {
            process.Start();

            if (process.StartInfo.RedirectStandardError && process.isErrorEventRaised)
                process.BeginErrorReadLine();

            if (process.StartInfo.RedirectStandardOutput && process.isOutputEventRaised)
                process.BeginOutputReadLine();


            List<Task> tasks = new List<Task>();

            if (process.StartInfo.RedirectStandardInput)
                tasks.Add(process.PipeInput());

            if (process.StartInfo.RedirectStandardOutput)
                tasks.Add(process.PipeOutput());

            Task.WaitAll(tasks.ToArray());

            return process;
        }

        private static Task PipeInput(this FFmpegProcess process)
        {
            if (process.Input == null)
                throw new Exception("Input set to null");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[4096];
                int bytesRead;

                while ((bytesRead = await process.Input.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await process.StandardInput.BaseStream.WriteAsync(bytes, 0, bytesRead);

                process.StandardInput.Close();
            });
        }

        private static Task PipeOutput(this FFmpegProcess process)
        {
            if (process.Output == null)
                throw new Exception("Output set to null");

            if (process.isOutputEventRaised)
                throw new InvalidOperationException("Cannot convert to stream with outputEvent raised");

            return Task.Run(async () =>
            {
                byte[] bytes = new byte[4096];
                int bytesRead;

                while ((bytesRead = await process.StandardOutput.BaseStream.ReadAsync(bytes, 0, bytes.Length)) != 0)
                    await process.Output.WriteAsync(bytes, 0, bytesRead);

            });
        }
    }
}
