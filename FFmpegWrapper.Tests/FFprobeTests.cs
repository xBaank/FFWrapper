using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using System.IO;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFprobeTests
    {
        private FFprobeClient ffProbeClient = new FFprobeClient();

        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(VideoFilesUri.MOV)]
        [InlineData(VideoFilesUri.OGG)]
        [InlineData(VideoFilesUri.MP4)]
        [InlineData(VideoFilesUri.AVI)]
        [InlineData(VideoFilesUri.WEBM)]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public void FileShouldGetFormat(string uri)
        {

        }
    }
}
