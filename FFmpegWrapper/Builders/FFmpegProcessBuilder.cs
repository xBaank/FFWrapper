using System;
using System.IO;

using FFmpegWrapper.Codecs;
using FFmpegWrapper.Extensions;
using FFmpegWrapper.Models;

namespace FFmpegWrapper.Builders
{
    public class FFmpegProcessBuilder : IFFmpegProcessBuilder<FFmpegProcessBuilder, FFmpegProcess>
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

        public FFmpegProcessBuilder RaiseOutputEvents(Action<FFmpegProcess, byte[]> action)
        {
            ffmpegProcess.OutputDataReceived += action;
            return this;
        }

        public FFmpegProcessBuilder RaiseErrorEvents(Action<FFmpegProcess, string> action)
        {
            ffmpegProcess.ErrorDataReceived += action;
            return this;
        }
        public FFmpegProcessBuilder RaiseExitErrorEvent(Action<FFmpegProcess> action)
        {
            ffmpegProcess.ExitedWithError += action;
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
            if(!string.IsNullOrWhiteSpace(args))
                ffmpegProcess.StartInfo.ArgumentList.AddRange(args.Trim().Split(" "));
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

        public FFmpegProcessBuilder SetInputBuffer(int value)
        {
            ffmpegProcess.InputBuffer = value;
            return this;
        }

        public FFmpegProcessBuilder SetOutputBuffer(int value)
        {
            ffmpegProcess.OutputBuffer = value;
            return this;
        }


        public FFmpegProcessBuilder AsRaw() => AddArguments("-f data");


        public FFmpegProcessBuilder To(IFormat type) =>
            RedirectOutput(true)
            .AddArguments(type.GetFormatArg())
            .AddArguments(type.GetCustomArgs())
            .AddArguments("pipe:");

        public FFmpegProcessBuilder To(Stream output, IFormat type) =>
            To(type)
            .SetOutput(output);

        public FFmpegProcessBuilder To(string output) =>
            RedirectOutput(false)
            .AddArguments(output);

        public FFmpegProcessBuilder From(IFormat type) =>
            AddArguments(type.GetFormatArg())
            .AddArguments(type.GetCustomArgs())
            .AddArguments("-i pipe:")
            .RedirectInput(true);

        public FFmpegProcessBuilder From(Stream input, IFormat type) =>
            From(type)
            .SetInput(input);

        public FFmpegProcessBuilder From(string input) =>
            AddArguments($"-i {input}")
            .RedirectInput(false);
    }
}

