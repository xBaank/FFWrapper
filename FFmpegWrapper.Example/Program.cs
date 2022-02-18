using FFmpegWrapper.Example;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;

using System.IO;
using System.Linq;

namespace WebmPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FFmpegClient fFmpegClient = new FFmpegClient();
            FFprobeClient fFprobeClient = new FFprobeClient();
            var song = YtUtils.GetSongsUrl("eldenring trailer").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();

            var format = fFprobeClient.GetMetadataAsync(streamInfo.Url).Result;
            double lastPostTime = 0;
            var duration = format.Duration + format.StartTime;
            while (lastPostTime < duration)
            {
                var packets = fFprobeClient.GetPacketsAsync(streamInfo.Url, StreamType.a, lastPostTime, 10).Result;
                lastPostTime = (double)(packets.LastOrDefault().DtsTime + packets.LastOrDefault().DurationTime);
            }


            var frames = fFprobeClient.GetFramesAsync(streamInfo.Url, StreamType.a, new MemoryStream()).Result;

            FileStream file = new FileStream("eldenring.mp3", FileMode.OpenOrCreate);

            fFmpegClient.ConvertToStream(streamInfo.Url, file, new Format(FormatTypes.MP3));

        }
    }
}
