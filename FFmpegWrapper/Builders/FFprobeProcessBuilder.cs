using System.IO;
using System;

using FFmpegWrapper.Models;
using FFmpegWrapper.Extensions;
using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Builders
{
    public class FFprobeProcessBuilder : IFFprobeProcessBuilder
    {
        private FFProbeProcess ffProbeProcess;

        public FFprobeProcessBuilder() => ffProbeProcess = new FFProbeProcess();

        public FFProbeProcess Build() => ffProbeProcess;

        public FFprobeProcessBuilder ShellExecute(bool value)
        {
            ffProbeProcess.StartInfo.UseShellExecute = value;
            return this;
        }

        public FFprobeProcessBuilder CreateNoWindow(bool value)
        {
            ffProbeProcess.StartInfo.CreateNoWindow = value;
            return this;
        }

        public FFprobeProcessBuilder Path(string value)
        {
            ffProbeProcess.StartInfo.FileName = value;
            return this;
        }

        public FFprobeProcessBuilder RaiseErrorEvents(Action<FFProbeProcess, string> action)
        {
            ffProbeProcess.ErrorDataReceived += action;
            return this;
        }
        public FFprobeProcessBuilder RaiseExitErrorEvent(Action<FFProbeProcess> action)
        {
            ffProbeProcess.ExitedWithError += action;
            return this;
        }

        public FFprobeProcessBuilder RedirectOutput(bool value)
        {
            ffProbeProcess.StartInfo.RedirectStandardOutput = value;
            return this;
        }

        public FFprobeProcessBuilder RedirectInput(bool value)
        {
            ffProbeProcess.StartInfo.RedirectStandardInput = value;
            return this;
        }

        public FFprobeProcessBuilder RedirectError(bool value)
        {
            ffProbeProcess.StartInfo.RedirectStandardError = value;
            return this;
        }

        public FFprobeProcessBuilder AddArguments(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                ffProbeProcess.StartInfo.ArgumentList.AddRange(args.Trim().Split(" "));
            return this;
        }

        public FFprobeProcessBuilder SetArguments(string args)
        {
            ffProbeProcess.StartInfo.Arguments = args;
            return this;
        }

        public FFprobeProcessBuilder SetInput(Stream stream)
        {
            ffProbeProcess.Input = stream;
            return this;
        }

        public FFprobeProcessBuilder SetInputBuffer(int value)
        {
            ffProbeProcess.InputBuffer = value;
            return this;
        }

        public FFprobeProcessBuilder From(string input) =>
            AddArguments(input);

        public FFprobeProcessBuilder From(Stream input) =>
            AddArguments("pipe:")
            .RedirectInput(true)
            .SetInput(input);

        public FFprobeProcessBuilder SelectStreams(StreamType streamType, int streamNumber = 0) =>
          AddArguments($"-select_streams {streamType}:{streamNumber}");

        public FFprobeProcessBuilder ShowPackets() =>
          AddArguments($"-show_packets");

        public FFprobeProcessBuilder ShowFrames() =>
          AddArguments($"-show_frames");

        public FFprobeProcessBuilder Reconnect() =>
           AddArguments("-reconnect 1");

        public FFprobeProcessBuilder ShowFormat() =>
            AddArguments("-show_format");

        public FFprobeProcessBuilder AsJson() =>
            AddArguments("-of json");

    }
}
