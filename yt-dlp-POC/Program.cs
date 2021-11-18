using Concentus.Structs;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using WebmOpus;

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
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds);
                        stream = YtDownloader.DownloadSong(args[0]).GetAwaiter().GetResult();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds);
                        WebmToOpus opus = new WebmToOpus(stream);
                        //Thread.Sleep(3000);
                        //opus.SeekToTimeStamp(173200);
                        //List<OpusPacket> opusPackets = opus.GetPackets(stream);
                        while(opus.Clusters.Count == 0)
                        stopwatch.Stop();
                        Console.WriteLine(stopwatch.ElapsedMilliseconds);
                        while (!stream.HasFinished) { }
                        byte[] pcmBufferBytes = WebmToOpus.GetPcm(opus.OpusContent, opus.OpusFormat);
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
