using System;
using System.Diagnostics;
using System.Text;

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
        protected K _builder;

        public string Path { get; protected set; }

        public Client(string path)
        {
            Path = path;
            _builder = Activator.CreateInstance<K>().CreateFFBuilder(path);
        }

        public event Action<Client<T, K>, Process, byte[]>? OutputReceived;
        public event Action<Client<T, K>, Process, string>? ErrorReceived;
        public event Action<Client<T, K>, Process>? ExitedWithError;

        public T PipeError(StringBuilder stringbuilder)
        {
            _builder.SetError(stringbuilder);
            return (T)this;
        }

        public T SetInputBuffer(int bufferSize)
        {
            _builder.SetInputBuffer(bufferSize);
            return (T)this;
        }

        public T SetOutputBuffer(int bufferSize)
        {
            _builder.SetOutputBuffer(bufferSize);
            return (T)this;
        }

        protected void ErrorRecieved(Process sender, string message) => ErrorReceived?.Invoke(this, sender, message);
        protected void OutputRecieved(Process sender, byte[] bytes) => OutputReceived?.Invoke(this, sender, bytes);
        protected void ExitWithErrorRecieved(Process sender) => ExitedWithError?.Invoke(this, sender);
    }
}
