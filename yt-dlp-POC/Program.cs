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

namespace yt_dlp_POC
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
                        YtStream stream = new YtStream(YtStream.GetSongUrl(args[0]).Result);
                        WebmToOpus opus = new WebmToOpus(stream);
                        List<OpusPacket> opusPackets = new List<OpusPacket>();
                        opus.DownloadClusterPositions().Wait();

                        foreach(var clusterPos in opus.ClusterPositions)
                        {
                            var cluster = opus.DownloadCluster(clusterPos).GetAwaiter().GetResult();
                            opusPackets.AddRange(cluster.Packets);
                        }

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
