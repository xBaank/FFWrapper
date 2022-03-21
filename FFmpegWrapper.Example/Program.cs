using FFmpegWrapper.Example;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

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
            var format = fFprobeClient.SetOutputBuffer(1024).PipeError(stringBuilder).GetMetadataAsync(streamInfo.Url).Result;
            double lastPostTime = 0;
            var duration = format.Result?.Duration + format.Result?.StartTime;
            while (lastPostTime < duration)
            {
                var packets = fFprobeClient.GetFramesAsync(streamInfo.Url, StreamType.a, lastPostTime, 10).Result;
                lastPostTime = (double)(packets.Result?.LastOrDefault().PktDtsTime + packets.Result?.LastOrDefault().PktDurationTime);
            }

            CancellationTokenSource cancellation = new CancellationTokenSource();

            MemoryStream memoryStream = new MemoryStream();
            var result = fFmpegClient.PipeError(stringBuilder)
                .WithCancellationToken(cancellation.Token)
                .ConvertToStreamAsync(
                streamInfo.Url,
                memoryStream,
                o => o.WithFormat(FormatTypes.MP3)
            );
            var b = result.Result;


        }
    }
}
