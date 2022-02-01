
using System.Diagnostics;
using System;

using WebmOpus.Models;
using System.IO;

namespace WebmOpus.Builders
{
    public static class FFmpegProcessBuilder
    {
        public static FFmpegProcess Build() => new FFmpegProcess();

        public static FFmpegProcess ShellExecute(this FFmpegProcess fFmpegProcess, bool value)
        {
            fFmpegProcess.StartInfo.UseShellExecute = value;
            return fFmpegProcess;
        }

        public static FFmpegProcess CreateNoWindow(this FFmpegProcess fFmpegProcess, bool value)
        {
            fFmpegProcess.StartInfo.CreateNoWindow = value;
            return fFmpegProcess;
        }

        public static FFmpegProcess Path(this FFmpegProcess fFmpegProcess, string value)
        {
            fFmpegProcess.StartInfo.FileName = value;
            return fFmpegProcess;
        }

        public static FFmpegProcess RaiseOutputEvents(this FFmpegProcess fFmpegProcess, Action<object, DataReceivedEventArgs> action)
        {
            fFmpegProcess.OutputDataReceived += new DataReceivedEventHandler(action);
            fFmpegProcess.isOutputEventRaised = true;
            return fFmpegProcess;
        }

        public static FFmpegProcess RaiseErrorEvents(this FFmpegProcess fFmpegProcess, Action<object, DataReceivedEventArgs> action)
        {
            fFmpegProcess.ErrorDataReceived += new DataReceivedEventHandler(action);
            fFmpegProcess.isErrorEventRaised = true;
            return fFmpegProcess;
        }

        public static FFmpegProcess RedirectOutput(this FFmpegProcess fFmpegProcess, bool value)
        {
            fFmpegProcess.StartInfo.RedirectStandardOutput = true;
            return fFmpegProcess;
        }

        public static FFmpegProcess RedirectInput(this FFmpegProcess fFmpegProcess, bool value)
        {
            fFmpegProcess.StartInfo.RedirectStandardInput = true;
            return fFmpegProcess;
        }

        public static FFmpegProcess RedirectError(this FFmpegProcess fFmpegProcess, bool value)
        {
            fFmpegProcess.StartInfo.RedirectStandardError = true;
            return fFmpegProcess;
        }

        public static FFmpegProcess AddArguments(this FFmpegProcess fFmpegProcess, string args)
        {
            fFmpegProcess.StartInfo.Arguments += args;
            return fFmpegProcess;
        }

        public static FFmpegProcess SetArguments(this FFmpegProcess fFmpegProcess, string args)
        {
            fFmpegProcess.StartInfo.Arguments = args;
            return fFmpegProcess;
        }

        public static FFmpegProcess SetInput(this FFmpegProcess fFmpegProcess, Stream stream)
        {
            fFmpegProcess.Input = stream;
            return fFmpegProcess;
        }

        public static FFmpegProcess SetOutput(this FFmpegProcess fFmpegProcess, Stream stream)
        {
            fFmpegProcess.Output = stream;
            return fFmpegProcess;
        }

    }
}
