using System;
using System.IO;

namespace FFmpegWrapper.Builders
{
    /// <inheritdoc/>
    public interface IFFmpegProcessBuilder<T, K> : IProcessBuilder<T, K>
    {
        public T SetInput(Stream stream);
        public T SetOutput(Stream stream);
        public T SetInputBuffer(int value);
        public T SetOutputBuffer(int value);
        public T RaiseOutputEvents(Action<object, byte[]> action);
        public T RaiseErrorEvents(Action<object, string> action);
    }
}
