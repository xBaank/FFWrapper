using System;
using System.IO;

using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFmpegVideoTests
    {

        private FFmpegClient fFmpegClient = new FFmpegClient();

        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(VideoFilesUri.MOV)]
        [InlineData(VideoFilesUri.OGG)]
        [InlineData(VideoFilesUri.MP4)]
        [InlineData(VideoFilesUri.AVI)]
        [InlineData(VideoFilesUri.WEBM)]
        public async void VideoShouldConvertToFile(string uri)
        {
            //Arrange
            Stream file;
            string saveFile = Guid.NewGuid().ToString() + ".mkv";

            //Act
            await fFmpegClient.ConvertAsync(uri, saveFile);
            file = File.Open(Path.Combine(Directory.GetCurrentDirectory(), saveFile), FileMode.Open);

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(VideoFilesUri.MOV)]
        [InlineData(VideoFilesUri.OGG)]
        [InlineData(VideoFilesUri.MP4)]
        [InlineData(VideoFilesUri.AVI)]
        [InlineData(VideoFilesUri.WEBM)]
        public async void VideoShouldConvertToStream(string uri)
        {
            //Arrange
            Stream file;

            //Act
            file = new MemoryStream();
            await fFmpegClient.ConvertToStreamAsync(uri, file, new Format(FormatTypes.MATROSKA));

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(VideoFilesUri.MOV)]
        [InlineData(VideoFilesUri.OGG)]
        [InlineData(VideoFilesUri.MP4)]
        [InlineData(VideoFilesUri.AVI)]
        [InlineData(VideoFilesUri.WEBM)]
        public async void VideoShouldConvertToPipe(string uri)
        {
            //Arrange
            Stream file;

            //Act
            file = new MemoryStream();
            var process = fFmpegClient.ConvertToPipe(uri, new Format(FormatTypes.MATROSKA));

            while (!process.HasExited)
                file.Write(await process.GetNextBytes());

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }
    }
}
