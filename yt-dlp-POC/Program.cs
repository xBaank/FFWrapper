using Concentus.Structs;
using System;
using System.IO;

namespace yt_dlp_POC
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                Stream stream;
                switch (args[0])
                {
                    case "-h":
                        PrintHelp();
                        break;
                    default:
                        stream = yt_dlp.DownloadSong(args[0]);
                        var a = OpusToPcm.GetPcm(stream);
                        break;
                }
                
            }
            else
            {
                PrintHelp();
            }
            
        }
        private static void PrintHelp()
        {
            Console.WriteLine("yt-dlp-POC [Query] [Output]");
        }
    }
}
