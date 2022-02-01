using System;
using System.Diagnostics;
using System.IO;
using System.Text;

using WebmOpus.Builders;

namespace WebmOpus.Models
{
    public partial class FFmpegClient
    {
        public string Path { get; }
        public event Func<object, byte[]>? OutputReceived;
        public event Action<string>? ErrorReceived;
        public FFmpegClient(string ffmpegPath) => Path = System.IO.Path.GetFullPath(ffmpegPath);

        public void ConvertToStream(string input, Stream output, MediaTypes outputType) => CreateFFmpegBuilder()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input)
            .ToStream(output, outputType)
            .Build()
            .StartProcess();

        public void ConvertToStream(Stream input, MediaTypes inputType, Stream output, MediaTypes outputType) => CreateFFmpegBuilder()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input, inputType)
            .ToStream(output, outputType)
            .Build()
            .StartProcess();

        public void Convert(string input, string output) => CreateFFmpegBuilder()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input)
            .To(output)
            .Build()
            .StartProcess();

        public void Convert(Stream input, string output, MediaTypes inputType) => CreateFFmpegBuilder()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input, inputType)
            .To(output)
            .Build()
            .StartProcess();

        public void ConvertRaisingEvents(string input) => CreateFFmpegBuilder()
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseOutputEvents(OutputRecieved)
            .From(input)
            .Build()
            .StartProcess();

        public void ConvertRaisingEvents(Stream input, MediaTypes inputType) => CreateFFmpegBuilder()
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseOutputEvents(OutputRecieved)
            .From(input, inputType)
            .Build()
            .StartProcess();

        private FFmpegProcessBuilder CreateFFmpegBuilder() => new FFmpegProcessBuilder()
                .ShellExecute(false)
                .CreateNoWindow(true)
                .Path(Path);


        private void OutputRecieved(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data is null)
                return;
            Console.WriteLine(dataReceivedEventArgs.Data);

            OutputReceived?.Invoke(Encoding.ASCII.GetBytes(dataReceivedEventArgs.Data));

        }

        private void ErrorRecieved(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data is null)
                return;
            Console.WriteLine(dataReceivedEventArgs.Data);

            ErrorReceived?.Invoke(dataReceivedEventArgs.Data);
        }
    }

}
