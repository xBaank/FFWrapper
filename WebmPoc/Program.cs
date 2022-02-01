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
            FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
            var song = YtUtils.GetSongsUrl("9bZkp7q19f0").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            var filea = File.Open("asd.mp3", FileMode.Open);
            FileStream file = new FileStream("asd.mp4", FileMode.OpenOrCreate);
            fFmpegClient.ConvertToStream(filea, MediaTypes.MP3, file, MediaTypes.MP4);

        }
    }
}
