using System.Diagnostics;
using System.IO;
using System.Text;

namespace WebmOpus.Extensions
{
    public static class ProcessExtensions
    {
        internal static Process WaitExit(this Process process)
        {
            if(process.HasExited)
                return process;
 
            process.WaitForExit();
            return process;
        }

        internal static Stream ToStream(this Process process)
        {
            if (!process.StartInfo.RedirectStandardOutput)
                throw new System.Exception("Output not being redirected");

            MemoryStream memory = new MemoryStream();

            process.WaitExit();
            while(!process.StandardOutput.EndOfStream)
            {
                var bytes = Encoding.ASCII.GetBytes(process.StandardOutput.ReadToEnd());
                memory.Write(bytes, 0, bytes.Length);
            }
            return memory;

        }

        internal static  Process StartProcess(this Process process)
        {
            process.StartInfo.RedirectStandardError = true;
            process.Start();
            process.BeginErrorReadLine();
            if (process.StartInfo.RedirectStandardOutput)
                process.BeginOutputReadLine();

            return process;
        }
    }
}
