using System;
using System.Diagnostics;
using System.Text;
using System.Threading;

using FFmpegWrapper.Builders;

namespace FFmpegWrapper.Models
{
    /// <summary>
    /// Abstract class for FFmpeg and FFprobe clients
    /// </summary>
    /// <typeparam name="T">Implementation type</typeparam>
    /// <typeparam name="K">Builder Type</typeparam>
    public abstract class Client<T, K> where K : FFProcessBuilder<K>, new() where T : Client<T, K>
    {

        protected string Path { get; }

        protected Client(string path) => Path = path;

        public event Action<Client<T, K>, Process, byte[]>? OutputReceived;
        public event Action<Client<T, K>, Process, string>? ErrorReceived;
        public event Action<Client<T, K>, Process>? ExitedWithError;

        public T PipeError(StringBuilder stringbuilder)
        {
            new K().CreateFFBuilder(Path).SetError(stringbuilder);
            return (T)this;
        }

        public T SetInputBuffer(int bufferSize)
        {
            new K().CreateFFBuilder(Path).SetInputBuffer(bufferSize);
            return (T)this;
        }

        public T SetOutputBuffer(int bufferSize)
        {
            new K().CreateFFBuilder(Path).SetOutputBuffer(bufferSize);
            return (T)this;
        }

        public T WithCancellationToken(CancellationToken cancellationToken)
        {
            new K().CreateFFBuilder(Path).SetCancellationToken(cancellationToken);
            return (T)this;
        }

        protected void ErrorRecieved(Process sender, string message) => ErrorReceived?.Invoke(this, sender, message);
        protected void OutputRecieved(Process sender, byte[] bytes) => OutputReceived?.Invoke(this, sender, bytes);
        protected void ExitWithErrorRecieved(Process sender) => ExitedWithError?.Invoke(this, sender);
    }
}
