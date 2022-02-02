
using System.Diagnostics;
using System;

using WebmOpus.Models;
using System.IO;
using WebmOpus.Extensions;

namespace WebmOpus.Builders
{
    public class FFmpegProcessBuilder
    {
        private FFmpegProcess ffmpegProcess;
        public FFmpegProcessBuilder() => ffmpegProcess = new FFmpegProcess();
        public FFmpegProcess Build() => ffmpegProcess;

        public FFmpegProcessBuilder ShellExecute(bool value)
        {
            ffmpegProcess.StartInfo.UseShellExecute = value;
            return this;
        }

        public FFmpegProcessBuilder CreateNoWindow(bool value)
        {
            ffmpegProcess.StartInfo.CreateNoWindow = value;
            return this;
        }

        public FFmpegProcessBuilder Path(string value)
        {
            ffmpegProcess.StartInfo.FileName = value;
            return this;
        }

        public FFmpegProcessBuilder RaiseOutputEvents(Action<object, DataReceivedEventArgs> action)
        {
            ffmpegProcess.OutputDataReceived += new DataReceivedEventHandler(action);
            ffmpegProcess.isOutputEventRaised = true;
            return this;
        }

        public FFmpegProcessBuilder RaiseErrorEvents(Action<object, DataReceivedEventArgs> action)
        {
            ffmpegProcess.ErrorDataReceived += new DataReceivedEventHandler(action);
            ffmpegProcess.isErrorEventRaised = true;
            return this;
        }

        public FFmpegProcessBuilder RedirectOutput(bool value)
        {
            ffmpegProcess.StartInfo.RedirectStandardOutput = value;
            return this;
        }

        public FFmpegProcessBuilder RedirectInput(bool value)
        {
            ffmpegProcess.StartInfo.RedirectStandardInput = value;
            return this;
        }

        public FFmpegProcessBuilder RedirectError(bool value)
        {
            ffmpegProcess.StartInfo.RedirectStandardError = value;
            return this;
        }

        public FFmpegProcessBuilder AddArguments(string args)
        {
            ffmpegProcess.StartInfo.Arguments += $" {args}";
            return this;
        }

        public FFmpegProcessBuilder SetArguments(string args)
        {
            ffmpegProcess.StartInfo.Arguments = args;
            return this;
        }

        public FFmpegProcessBuilder SetInput(Stream stream)
        {
            ffmpegProcess.Input = stream;
            return this;
        }

        public FFmpegProcessBuilder SetOutput(Stream stream)
        {
            ffmpegProcess.Output = stream;
            return this;
        }

        public FFmpegProcessBuilder To(Stream output, MediaTypes type) =>
            RedirectOutput(true)
            .AddArguments($"-f {type.GetArgs()} pipe:")
            .SetOutput(output);

        public FFmpegProcessBuilder To(string output) =>
            RedirectOutput(false)
            .AddArguments($" {output}");

        public FFmpegProcessBuilder From(Stream input, MediaTypes type) =>
            AddArguments($"-f {type.GetArgs()} -i pipe:")
            .RedirectInput(true)
            .SetInput(input);

        public FFmpegProcessBuilder From(string input) =>
            AddArguments($"-i {input}")
            .RedirectInput(false);
    }
}

