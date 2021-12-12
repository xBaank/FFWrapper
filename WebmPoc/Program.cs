using Concentus.Structs;
using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebmOpus;
using WebmOpus.Models;
using YoutubeExplode;

namespace WebmPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-h":
                        PrintHelp();
                        break;
                    default:
                        YtStream stream = new YtStream("https://www.youtube.com/watch?v=mYEA5A0Bjyo");
                        WebmToOpus opus = new WebmToOpus(stream);
                        List<OpusPacket> opusPackets = new List<OpusPacket>();
                        opus.DownloadClusterPositions().Wait();
                        foreach(var clusterPos in opus.ClusterPositions)
                        {
                            long percentage = (long)(((float)clusterPos.ClusterPos / stream.Size) * 100);
                            Console.WriteLine($"{percentage}%");
                            var cluster = opus.DownloadCluster(clusterPos).GetAwaiter().GetResult();
                            opusPackets.AddRange(cluster.Packets);
                        }

                        //This part convert the opus into raw pcm and saves it into a wav file
                        //THIS IMPLEMENTATION TAKES TOO MUCH MEMORY USE!!!
                        Console.WriteLine("Converting...");
                        byte[] pcmBufferBytes = WebmToOpus.GetPcm(opusPackets, opus.OpusFormat);
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
