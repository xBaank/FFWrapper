using System;
using System.IO;

using FFmpegWrapper.Models;

namespace FFmpegWrapper.Builders
{
    /// <inheritdoc/>
    public interface IFFmpegProcessBuilder : IProcessBuilder<FFmpegProcessBuilder, FFmpegProcess>
    {
        public FFmpegProcessBuilder SetInput(Stream stream);
        public FFmpegProcessBuilder SetOutput(Stream stream);
        public FFmpegProcessBuilder SetInputBuffer(int value);
        public FFmpegProcessBuilder SetOutputBuffer(int value);
        public FFmpegProcessBuilder RaiseOutputEvents(Action<FFmpegProcess, byte[]> action);
        public FFmpegProcessBuilder RaiseErrorEvents(Action<FFmpegProcess, string> action);
    }
}
