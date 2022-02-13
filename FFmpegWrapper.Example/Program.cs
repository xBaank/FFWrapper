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
            FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
            FFprobeClient fFprobeClient = new FFprobeClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffprobe.exe");
            var song = YtUtils.GetSongsUrl("Alex al habla trailer").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            var format = fFprobeClient.GetMetadata(streamInfo.Url);

            FileStream file = new FileStream("eldenring.mp3", FileMode.OpenOrCreate);

            fFmpegClient.ConvertToStream(streamInfo.Url, file, new Format(MediaTypes.DATA));

        }
    }
}
