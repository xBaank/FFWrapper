using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using WebmOpus.Extensions;

namespace WebmOpus.Models
{
    public class FFmpegClient
    {
        public string Path { get; }
        public event Func<Object, byte[]>? OutputReceived;
        public event Action<string>? ErrorReceived;
        public FFmpegClient(string ffmpegPath) => Path = System.IO.Path.GetFullPath(ffmpegPath);

        public void ConvertToStream(string input, Stream output, MediaTypes outputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input).ToStream(output, outputType).StartProcess();

        public void ConvertToStream(Stream input, MediaTypes inputType, Stream output, MediaTypes outputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input,inputType).ToStream(output,outputType).StartProcess();

        public void Convert(string input, string output) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input).To(output).StartProcess();

        public void Convert(Stream input, string output, MediaTypes inputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input,inputType).To(output).StartProcess();

        public void ConvertRaisingEvents(string input) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved).RaiseOutputEvents(OutputRecieved)
            .From(input).Start();
        public void ConvertRaisingEvents(Stream input, MediaTypes inputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved).RaiseOutputEvents(OutputRecieved)
            .From(input,inputType).Start();

        private FFmpegProcess FFmpegProcess()
        {
            FFmpegProcess process = new FFmpegProcess();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Path;

            return process;
        }

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
