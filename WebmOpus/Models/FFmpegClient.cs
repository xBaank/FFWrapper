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
        public event Func<Object, byte[]> Output;
        public event Action<string> Error;
        public FFmpegClient(string ffmpegPath) => Path = System.IO.Path.GetFullPath(ffmpegPath);

        public Stream ConvertToStream(string input) => Conversion(input);
        public Stream ConvertToStream(Stream input) => Conversion(input);
        public void Convert(string input, string output) => Conversion(input, output);
        public void Convert(Stream input, string output) => Conversion(input, output);


        internal Stream Conversion<T>(T input)
        {
            if (input.IsNullOrWhiteSpaced())
                throw new ArgumentNullException("Input cannot be null or whitespaced");

            return FFmpegProcess<T>(input, default).StartProcess().WaitExit().ToStream();

        }

        internal void Conversion<T>(T input, string output)
        {
            if (input.IsNullOrWhiteSpaced())
                throw new ArgumentNullException("Input cannot be null or whitespaced");

            FFmpegProcess<T>(input, output).StartProcess().WaitExit();

        }

        private Process FFmpegProcess<I>(I input, string? output = default)
        {

            bool isStreamInput = !input.IsString();
            bool isStreamOutput = output.IsNullOrWhiteSpaced();
            string inputArgs = isStreamInput ? $"-i pipe:" : $"-i '{input}'";
            string outputArgs = isStreamOutput ? "-f opus pipe:" : $"-o {output}";


            Process process = new Process();

            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.FileName = Path;
            process.StartInfo.Arguments = $"{inputArgs} {outputArgs}";
            process.StartInfo.RedirectStandardOutput = isStreamOutput;
            process.StartInfo.RedirectStandardInput = isStreamInput;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += OutputRecieved;
            process.ErrorDataReceived += ErrorRecieved;

            //TODO: piping to input if it's stream type

            return process;
        }
        private void OutputRecieved(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data is null)
                return;

            Output?.Invoke(Encoding.ASCII.GetBytes(dataReceivedEventArgs.Data));

        }
        private void ErrorRecieved(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            if (dataReceivedEventArgs.Data is null)
                return;

            Error?.Invoke(dataReceivedEventArgs.Data);
        }
    }

}
