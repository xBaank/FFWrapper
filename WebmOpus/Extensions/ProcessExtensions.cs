using System;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;
using WebmOpus.Models;

namespace WebmOpus.Extensions
{
    public static class ProcessExtensions
    {

        internal static Stream ToStream(this FFmpegProcess process,MediaTypes type)
        {
            if (process.isOutputEventRaised)
                throw new InvalidOperationException("Cannot convert to stream with outputEvent raised");

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.Arguments += $" -f {type} pipe:";

            process.StartProcess();

            return process.Output;
        }

        internal static void To(this FFmpegProcess process, string output)
        {
            process.StartInfo.RedirectStandardOutput = false;
            process.StartInfo.Arguments += $" -o {output}";

            process.StartProcess();
        }

        internal static FFmpegProcess From(this FFmpegProcess process, Stream input,MediaTypes type)
        {
            process.StartInfo.Arguments = $"-f {type} -i pipe:";
            process.StartInfo.RedirectStandardInput = true;
            process.Input = input;

            return process;
        }

        internal static FFmpegProcess From(this FFmpegProcess process, string input)
        {
            process.StartInfo.Arguments = $"-i {input}";
            process.StartInfo.RedirectStandardInput = true; 
            return process;
        }

        internal static FFmpegProcess RaiseOutputEvents(this FFmpegProcess process, Action<Object, DataReceivedEventArgs> action)
        {
            process.StartInfo.RedirectStandardOutput = true;
            process.isOutputEventRaised = true;
            process.OutputDataReceived += new DataReceivedEventHandler(action);
            return process;
        }

        internal static FFmpegProcess RaiseErrorEvents(this FFmpegProcess process, Action<Object, DataReceivedEventArgs> action)
        {
            process.StartInfo.RedirectStandardError = true;
            process.isErrorEventRaised = true;
            process.ErrorDataReceived += new DataReceivedEventHandler(action);
            return process;
        }

        internal static FFmpegProcess StartProcess(this FFmpegProcess process)
        {
            process.Start();

            if (process.StartInfo.RedirectStandardError && process.isErrorEventRaised)
                process.BeginErrorReadLine();

            if (process.StartInfo.RedirectStandardOutput && process.isOutputEventRaised)
                process.BeginOutputReadLine();

            //TODO: avoid deadlock
            if (process.Input != null)
            {
                MemoryStream ms = new MemoryStream();
                byte[] bytes = new byte[4096];
                int bytesToWrite;

                while ((bytesToWrite = process.Input.Read(bytes, 0, 4096)) != 0)
                {
                    process.StandardInput.BaseStream.Write(bytes, 0, bytesToWrite);
                    process.StandardInput.BaseStream.Flush();
                }
                process.Output = ms;
            }
                    

            return process;
        }
    }
}
