using FFmpegWrapper.Arguments;
using FFmpegWrapper.Example;
using FFmpegWrapper.Models;

using System;
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
            FileStream file = new FileStream("eldenring.aac", FileMode.OpenOrCreate);
            var process = fFmpegClient.ConvertToPipe(streamInfo.Url, new CopyCodecFormat(type: "Data", args: TracksArguments.WithAllAudioTracks()));
            fFmpegClient.ExitedWithError += ErrorExit;
            byte[] buffer;
            while ((buffer = process.GetNextBytes().Result).Length > 0 && !process.HasExited)
            {
                file.Write(buffer, 0, buffer.Length);
            }


        }

        private static void ErrorExit(FFmpegClient ffmpegClient, FFmpegProcess ffmpegProcess)
        {
            Console.WriteLine($"Exited with error code {ffmpegProcess.ExitCode}");
        }
    }
}
