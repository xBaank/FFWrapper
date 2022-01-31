using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using WebmOpus;
using WebmOpus.Models;
using WebmPOC;

namespace WebmPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory()+"/FFMPEG/ffmpeg.exe");
            var song = YtUtils.GetSongsUrl("9bZkp7q19f0").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            var filea = File.Open("asd.mp3", FileMode.Open);
            Stream stream1 = fFmpegClient.ConvertToStream(filea,MediaTypes.MP3,MediaTypes.OPUS);
            FileStream file = new FileStream("asd.webm", FileMode.OpenOrCreate);
            stream1.CopyTo(file);
            
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "-h":
                        PrintHelp();
                        break;
                    default:
                        song = YtUtils.GetSongsUrl("1b3FAg2j0As").GetAwaiter().GetResult();
                        streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
                        WebmOpusStream stream = new WebmOpusStream(streamInfo.Url);
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
                        byte[] pcmBufferBytes = opus.GetPcm(opusPackets);
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
