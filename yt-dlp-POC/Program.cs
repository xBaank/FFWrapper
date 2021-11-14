using Concentus.Structs;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

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
                        WebmOpus opus = new WebmOpus(stream);
                        //List<OpusPacket> opusPackets = opus.GetPackets(stream);
                        while (!stream.HasFinished) { }
                        byte[] pcmBufferBytes = WebmOpus.GetPcm(opus.OpusContent, opus.OpusFormat);
                        MemoryStream memoryStream = new MemoryStream(pcmBufferBytes);
                        var rawSourceWaveStream = new RawSourceWaveStream(pcmBufferBytes, 0, pcmBufferBytes.Length, new WaveFormat((int)opus.OpusFormat.sampleFrequency, opus.OpusFormat.channels));
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
