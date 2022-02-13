using System;
using System.Diagnostics;

namespace FFmpegWrapper.Models
{
    public abstract class Client
    {
        public string Path { get; protected set; }

        public event Action<Client, Process, byte[]>? OutputReceived;
        public event Action<Client, Process, string>? ErrorReceived;
        public event Action<Client, Process>? ExitedWithError;

        protected void ErrorRecieved(Process sender, string message) => ErrorReceived?.Invoke(this, sender, message);
        protected void OutputRecieved(Process sender, byte[] bytes) => OutputReceived?.Invoke(this, sender, bytes);
        protected void ExitWithErrorRecieved(Process sender) => ExitedWithError?.Invoke(this, sender);
    }
}
