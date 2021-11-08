using Concentus.Structs;
using NAudio.Wave;
using System;
using System.Collections.Generic;
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
                        List<OpusPacket> opusPackets = OpusToPcm.GetPackets(stream);
                        byte[] pcmBufferBytes = OpusToPcm.GetPcm(opusPackets);
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
