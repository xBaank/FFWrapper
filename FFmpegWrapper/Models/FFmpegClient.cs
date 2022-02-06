﻿using System;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;

namespace FFmpegWrapper.Models
{
    public partial class FFmpegClient
    {
        public string Path { get; }
        public event Action<FFmpegClient, FFmpegProcess, byte[]>? OutputReceived;
        public event Action<FFmpegClient, FFmpegProcess, string>? ErrorReceived;

        public FFmpegClient(string ffmpegPath) => Path = System.IO.Path.GetFullPath(ffmpegPath);

        public void ConvertToStream(string input, Stream output, MediaTypes outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input)
            .To(output, outputType)
            .Build()
            .Start();

        public Task ConvertToStreamAsync(string input, Stream output, MediaTypes outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input)
            .To(output, outputType)
            .Build()
            .StartAsync();

        public void ConvertToStream(Stream input, MediaTypes inputType, Stream output, MediaTypes outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input, inputType)
            .To(output, outputType)
            .Build()
            .Start();

        public Task ConvertToStreamAsync(Stream input, MediaTypes inputType, Stream output, MediaTypes outputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input, inputType)
            .To(output, outputType)
            .Build()
            .StartAsync();

        public void Convert(string input, string output) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input)
            .To(output)
            .Build()
            .Start();

        public Task ConvertAsync(string input, string output) => CreateFFmpegBuilder()
           .RedirectError(true)
           .RaiseErrorEvents(ErrorRecieved)
           .From(input)
           .To(output)
           .Build()
           .StartAsync();

        public void Convert(Stream input, string output, MediaTypes inputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input, inputType)
            .To(output)
            .Build()
            .Start();

        public Task ConvertAsync(Stream input, string output, MediaTypes inputType) => CreateFFmpegBuilder()
            .RedirectError(true)
            .RaiseErrorEvents(ErrorRecieved)
            .From(input, inputType)
            .To(output)
            .Build()
            .StartAsync();

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
    }

}
