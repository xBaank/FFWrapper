using System.IO;
using System.Linq;
using WebmOpus.Models;
using WebmPOC;

namespace WebmPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
            var song = YtUtils.GetSongsUrl("9bZkp7q19f0").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            //var filea = File.Open("asd.webm", FileMode.Open);
            FileStream file = new FileStream("asd.mp3", FileMode.OpenOrCreate);
            fFmpegClient.ConvertToStream(streamInfo.Url, file, MediaTypes.MP3);

        }
    }
}
