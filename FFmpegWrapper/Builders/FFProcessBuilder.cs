using FFmpegWrapper.Models;
using System.IO;
using System;
using FFmpegWrapper.Extensions;

namespace FFmpegWrapper.Builders
{
    /// <summary>
    /// Abstract class for FFmpeg and FFprobe processes.
    /// </summary>
    /// <typeparam name="T">Class implementation</typeparam>
    public abstract class FFProcessBuilder<T> where T : FFProcessBuilder<T>
    {
        private FFProcess ffProcess = new FFProcess();

        public FFProcess Build() => ffProcess;

        public T ShellExecute(bool value)
        {
            ffProcess.StartInfo.UseShellExecute = value;

            return (T)this;
        }

        public T CreateNoWindow(bool value)
        {
            ffProcess.StartInfo.CreateNoWindow = value;
            return (T)this;
        }
        public T Path(string value)
        {
            ffProcess.StartInfo.FileName = value;
            return (T)this;
        }

        public T RaiseErrorEvents(Action<FFProcess, string> action)
        {
            ffProcess.ErrorDataReceived += action;
            return (T)this;
        }
        public T RaiseExitErrorEvent(Action<FFProcess> action)
        {
            ffProcess.ExitedWithError += action;
            return (T)this;
        }

        public T RedirectOutput(bool value)
        {
            ffProcess.StartInfo.RedirectStandardOutput = value;
            return (T)this;
        }

        public T RedirectInput(bool value)
        {
            ffProcess.StartInfo.RedirectStandardInput = value;
            return (T)this;
        }

        public T RedirectError(bool value)
        {
            ffProcess.StartInfo.RedirectStandardError = value;
            return (T)this;
        }

        public T AddArguments(string args)
        {
            if (!string.IsNullOrWhiteSpace(args))
                ffProcess.StartInfo.ArgumentList.AddRange(args.Trim().Split(" "));
            return (T)this;
        }

        public T SetInput(Stream stream)
        {
            ffProcess.Input = stream;
            return (T)this;
        }

        public T SetOutput(Stream stream)
        {
            ffProcess.Output = stream;
            return (T)this;
        }

        public abstract T From(string input);
        public abstract T To(string output);

        /// <summary>
        /// Set the process path, with no window and no shell execute
        /// </summary>
        public T CreateFFBuilder(string path)
        {
            ffProcess.Dispose();
            ffProcess = new FFProcess();
            return ShellExecute(false).CreateNoWindow(true).Path(path);
        }
    }
}
