using System;
using System.IO;

using FFmpegWrapper.Models;

namespace FFmpegWrapper.Builders
{
    /// <inheritdoc/>
    public interface IFFprobeProcessBuilder : IProcessBuilder<FFprobeProcessBuilder, FFProbeProcess>
    {
        public FFprobeProcessBuilder SetInput(Stream stream);
        public FFprobeProcessBuilder SetInputBuffer(int value);
        public FFprobeProcessBuilder RaiseErrorEvents(Action<FFProbeProcess, string> action);
    }
}
