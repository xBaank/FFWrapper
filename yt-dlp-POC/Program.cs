using Concentus.Structs;
using NAudio.Wave;
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
                YtStream stream;
                switch (args[0])
                {
                    case "-h":
                        PrintHelp();
                        break;
                    default:
                        stream = YtDownloader.DownloadSong(args[0]).Result;
                        short[] pcmBuffer = OpusToPcm.GetPcm(stream);
                        byte[] pcmBufferBytes = new byte[pcmBuffer.Length * 2];
                        Buffer.BlockCopy(pcmBuffer,0,pcmBufferBytes,0,pcmBufferBytes.Length);
                        MemoryStream memoryStream = new MemoryStream(pcmBufferBytes);
                        var rawSourceWaveStream = new RawSourceWaveStream(pcmBufferBytes,0,pcmBufferBytes.Length,new WaveFormat(48000,2));
                        WaveFileWriter.CreateWaveFile("output.wav", rawSourceWaveStream);
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
