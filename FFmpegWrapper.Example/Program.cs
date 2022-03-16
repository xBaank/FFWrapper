using FFmpegWrapper.Example;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;

using System.IO;
using System.Linq;
using System.Text;

namespace WebmPoc
{
    public class Program
    {
        public static void Main()
        {
            FFmpegClient fFmpegClient = new FFmpegClient();
            FFprobeClient fFprobeClient = new FFprobeClient();
            var song = YtUtils.GetSongsUrl("eldenring trailer").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            StringBuilder stringBuilder = new StringBuilder();
            //var format = fFprobeClient.SetOutputBuffer(1024).PipeError(stringBuilder).GetMetadataAsync(streamInfo.Url).Result;
            //double lastPostTime = 0;
            //var duration = format.Result?.Duration + format.Result?.StartTime;
            //while (lastPostTime < duration)
            //{
            //    var packets = fFprobeClient.GetPacketsAsync(streamInfo.Url, StreamType.a, lastPostTime, 10).Result;
            //    lastPostTime = (double)(packets.Result?.LastOrDefault().DtsTime + packets.Result?.LastOrDefault().DurationTime);
            //}


            FileStream file = new FileStream("eldenring.mp3", FileMode.OpenOrCreate);
            var result = fFmpegClient.PipeError(stringBuilder).ConvertToStreamAsync(
                streamInfo.Url,
                file,
                o => o.WithFormat(FormatTypes.MP3)
            ).Result;
            var a = stringBuilder.ToString();



        }
    }
}
