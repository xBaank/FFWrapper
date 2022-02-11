using FFmpegWrapper.Arguments;
using FFmpegWrapper.Example;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace WebmPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
            var song = YtUtils.GetSongsUrl("elden ring trailer").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            //var filea = File.Open("asd.webm", FileMode.Open);
            FileStream file = new FileStream("eldenring.mp3", FileMode.OpenOrCreate);
            var process = fFmpegClient.ConvertToPipe(streamInfo.Url, new CopyCodecFormat(MediaTypes.DATA, CocecFormatTypes.A, TracksArguments.WithAllAudioTracks()));
            fFmpegClient.ExitedWithError += ErrorExit;
            byte[] buffer;
            List<byte[]> packets = new List<byte[]>();
            while ((buffer = process.GetNextBytes().Result).Length > 0 && !process.HasExited)
            {
                file.Write(buffer, 0, buffer.Length);
                packets.Add(buffer);
            }
            buffer = packets.SelectMany(i => i).ToArray();
            Console.WriteLine("a");

        }

        private static void ErrorExit(FFmpegClient ffmpegClient, FFmpegProcess ffmpegProcess)
        {
            Console.WriteLine($"Exited with error code {ffmpegProcess.ExitCode}");
        }
    }
}
