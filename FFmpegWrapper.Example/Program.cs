using FFmpegWrapper.Arguments;
using FFmpegWrapper.Example;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;

namespace WebmPoc
{
    public class Program
    {
        public static void Main(string[] args)
        {
            FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
            var song = YtUtils.GetSongsUrl("Alex al habla trailer").GetAwaiter().GetResult();
            var streamInfo = YtUtils.GetStreamInfo(song.FirstOrDefault().Id).GetAwaiter().GetResult();
            //var filea = File.Open("asd.webm", FileMode.Open);
            FileStream file = new FileStream("eldenring.mp3", FileMode.OpenOrCreate);
            fFmpegClient.ConvertToStream(streamInfo.Url, file, new Format(MediaTypes.DATA));

        }
    }
}
