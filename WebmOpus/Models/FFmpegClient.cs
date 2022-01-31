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

        public Stream ConvertToStream(string input,MediaTypes outputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input).ToStream(outputType);

        public Stream ConvertToStream(Stream input,MediaTypes inputType,MediaTypes outputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input,inputType).ToStream(outputType);

        public void Convert(string input, string output) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input).To(output);

        public void Convert(Stream input, string output, MediaTypes inputType) => FFmpegProcess()
            .RaiseErrorEvents(ErrorRecieved)
            .From(input,inputType).To(output);

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

            FFmpegProcess process = sender as FFmpegProcess;

            if(process.StartInfo.RedirectStandardOutput)
                WriteToOutput(process);

            

            ErrorReceived?.Invoke(dataReceivedEventArgs.Data);
        }

        private void WriteToOutput(FFmpegProcess process)
        {
            MemoryStream ms = new MemoryStream();
            byte[] bytes = new byte[4096];
            int bytesToWrite;

            while ((bytesToWrite = process.StandardOutput.BaseStream.Read(bytes, 0, 4096)) != 0)
            {
                process.StandardInput.BaseStream.Write(bytes, 0, bytesToWrite);
                process.StandardInput.BaseStream.Flush();
            }
            process.Output = ms;
        }
    }

}
