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
        Stream stream;

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
            string saveFile = Guid.NewGuid().ToString() + ".mkv";

            //Act
            await fFmpegClient.ConvertAsync(uri, saveFile);
            stream = File.Open(saveFile, FileMode.Open);

            //Assert
            Assert.True(stream.Length > 0);

            stream.Dispose();
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

            //Act
            stream = new MemoryStream();
            await fFmpegClient.ConvertToStreamAsync(uri, stream, new Format(FormatTypes.MATROSKA));

            //Assert
            Assert.True(stream.Length > 0);

            stream.Dispose();
        }

    }
}
