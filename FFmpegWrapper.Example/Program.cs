using FFmpegWrapper.Example;
using FFmpegWrapper.Formats;
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
            FFprobeClient fFprobeClient = new FFprobeClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffprobe.exe");
            var song = YtUtils.GetSongsUrl("eldenring trailer").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            var format = fFprobeClient.GetMetadata(streamInfo.Url);
            double lastPostTime = 0;
            while (lastPostTime < format.Duration)
            {
                var packets = fFprobeClient.GetPackets(streamInfo.Url, StreamType.a, lastPostTime, 20);
                lastPostTime = (double)(packets.LastOrDefault().DtsTime + packets.LastOrDefault().DurationTime);
            }


            //var frames = fFprobeClient.GetFrames(streamInfo.Url, StreamType.a);

            FileStream file = new FileStream("eldenring.mp3", FileMode.OpenOrCreate);

            fFmpegClient.ConvertToStream(streamInfo.Url, file, new Format(FormatTypes.MP3));

        }
    }
}
