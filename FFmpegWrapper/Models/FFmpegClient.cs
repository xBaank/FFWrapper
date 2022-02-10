﻿using System;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;
using FFmpegWrapper.Codecs;

namespace FFmpegWrapper.Models
{
    public partial class FFmpegClient
    {
        public string Path { get; }
        public event Action<FFmpegClient, FFmpegProcess, byte[]>? OutputReceived;
        public event Action<FFmpegClient, FFmpegProcess, string>? ErrorReceived;
        public event Action<FFmpegClient, FFmpegProcess>? ExitedWithError;

        public FFmpegClient(string ffmpegPath) => Path = System.IO.Path.GetFullPath(ffmpegPath);

        public void ConvertToStream(string input, Stream output, IFormat outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output, outputType)
            .Build()
            .Start();

        public Task ConvertToStreamAsync(string input, Stream output, IFormat outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output, outputType)
            .Build()
            .StartAsync();

        public void ConvertToStream(Stream input, IFormat inputType, Stream output, IFormat outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(output, outputType)
            .Build()
            .Start();

        public Task ConvertToStreamAsync(Stream input, IFormat inputType, Stream output, IFormat outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(output, outputType)
            .Build()
            .StartAsync();

        public void Convert(string input, string output) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output)
            .Build()
            .Start();

        public Task ConvertAsync(string input, string output) => CreateFFmpegBuilder()
           .RedirectError(true)
           .RaiseErrorEvents(ErrorRecieved)
           .RaiseExitErrorEvent(ExitWithErrorRecieved)
           .From(input)
           .To(output)
           .Build()
           .StartAsync();

        public void Convert(Stream input, string output, IFormat inputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(output)
            .Build()
            .Start();

        public Task ConvertAsync(Stream input, string output, IFormat inputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(output)
            .Build()
            .StartAsync();

        /// <summary>
        /// Must read from output pipe using <see cref="FFmpegProcess.GetNextBytes"/>
        /// </summary>
        public FFmpegProcess ConvertToPipe(Stream input, IFormat inputType, IFormat outputType)
        {
            var process = CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(outputType)
            .Build();

            process.StartAsync();

            return process;

        }

        /// <summary>
        /// Must read from output pipe using <see cref="FFmpegProcess.GetNextBytes"/>
        /// </summary>
        public FFmpegProcess ConvertToPipe(string input, IFormat outputType)
        {
            var process = CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(outputType)
            .Build();

            process.StartAsync();

            return process;
        }

        private FFmpegProcessBuilder CreateFFmpegBuilder() => new FFmpegProcessBuilder()
            .ShellExecute(false)
            .CreateNoWindow(true)
            .Path(Path);

        private void ErrorRecieved(FFmpegProcess sender, string message)
        {
            ErrorReceived?.Invoke(this, sender, message);
            Console.WriteLine(message);
        }

        private void OutputRecieved(FFmpegProcess sender, byte[] bytes) => OutputReceived?.Invoke(this, sender, bytes);
        private void ExitWithErrorRecieved(FFmpegProcess sender) => ExitedWithError?.Invoke(this, sender);
    }

}
