using FFmpegWrapper.Models;
using System.IO;
using System;
using FFmpegWrapper.Extensions;
using System.Text;
using FFmpegWrapper.Utils;
using System.Threading;

namespace FFmpegWrapper.Builders
{
    /// <summary>
    /// Abstract class for FFmpeg and FFprobe processes.
    /// </summary>
    /// <typeparam name="T">Class implementation</typeparam>
    public abstract class FFProcessBuilder<T> where T : FFProcessBuilder<T>
    {
        private readonly FFProcess ffProcess = new FFProcess();

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
            ThrowUtils.ThrowFor<ArgumentNullException>(stream is null, "Input cannot be set to null");

            RedirectInput(true);
            ffProcess.Input = stream;
            return (T)this;
        }

        public T SetOutput(Stream stream)
        {
            ThrowUtils.ThrowFor<ArgumentNullException>(stream is null, "Output cannot be set to null");

            RedirectOutput(true);
            ffProcess.Output = stream;
            return (T)this;
        }

        public T SetError(StringBuilder builder)
        {
            ThrowUtils.ThrowFor<ArgumentNullException>(builder is null, "Builder cannot be set to null");

            RedirectError(true);
            ffProcess.Error = builder;
            return (T)this;
        }

        public T SetInputBuffer(int bufferSize)
        {
            ThrowUtils.ThrowFor<ArgumentException>(bufferSize <= 0, "BufferSize cannot be less or equal to 0");

            ffProcess.InputBuffer = bufferSize;
            return (T)this;
        }

        public T SetOutputBuffer(int bufferSize)
        {
            ThrowUtils.ThrowFor<ArgumentException>(bufferSize <= 0, "BufferSize cannot be less or equal to 0");

            ffProcess.OutputBuffer = bufferSize;
            return (T)this;
        }

        public T SetCancellationToken(CancellationToken cancellationToken)
        {
            ffProcess.cancellationToken = cancellationToken;
            return (T)this;
        }

        public abstract T From(string input);
        public abstract T To(string output);

        /// <summary>
        /// Set the process path, with no window and no shell execute
        /// </summary>
        public T CreateFFBuilder(string path)
        {
            ffProcess.StartInfo.ArgumentList.Clear();
            AddArguments("-hide_banner -loglevel error");
            return ShellExecute(false).CreateNoWindow(true).Path(path);
        }
    }
}
