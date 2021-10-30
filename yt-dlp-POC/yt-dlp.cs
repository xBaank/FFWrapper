using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace yt_dlp_POC
{
    public class yt_dlp
    {
        const string ARGUMENTS = @"-f bestaudio --no-part --max-downloads 1 --output ""{0}"" ";
        const string SEARCHARGUMENTS = @"""ytsearch:{0}""";
        static readonly string FILEPATH = Path.Combine(Directory.GetCurrentDirectory(), @"yt-dlp.exe");

        public static Stream DownloadSong(string query)
        {
            string allArguments;
            string path = Path.GetTempFileName();
            FileStream fileStream = File.Open(path, FileMode.OpenOrCreate, FileAccess.Read, FileShare.ReadWrite);
            bool result = Uri.TryCreate(query, UriKind.Absolute, out Uri uriResult)
                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
            if(result)
            {
                allArguments = string.Format(ARGUMENTS,path) + query;
            }
            else
            {
                allArguments = string.Format(ARGUMENTS, path) + string.Format(SEARCHARGUMENTS, query);
            }

            ProcessStartInfo processInfo = new ProcessStartInfo(FILEPATH,allArguments);
            processInfo.UseShellExecute = false;
            processInfo.RedirectStandardError = true;
            processInfo.RedirectStandardOutput = true;
            Process process = new Process();
            process.StartInfo = processInfo;
            process.ErrorDataReceived += new DataReceivedEventHandler(OnError);
            process.OutputDataReceived += new DataReceivedEventHandler(OnData);
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
            return fileStream;


        }
        private static void OnError(object sender,DataReceivedEventArgs dataReceivedEventArgs)
        {
            Console.WriteLine(dataReceivedEventArgs.Data);
        }
        private static void OnData(object sender, DataReceivedEventArgs dataReceivedEventArgs)
        {
            Console.WriteLine(dataReceivedEventArgs.Data);
        }
    }
}
