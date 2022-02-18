using System;
using System.IO;

using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFmpegVideoTests : IDisposable
    {

        private FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
        private Stream file;

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
            file = File.Open(Path.Combine(Directory.GetCurrentDirectory(), saveFile), FileMode.Open);

            //Assert
            Assert.True(file.Length > 0);

            Dispose();
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
            string saveFile = Guid.NewGuid().ToString() + ".mkv";

            //Act
            file = new FileStream(saveFile, FileMode.OpenOrCreate);
            await fFmpegClient.ConvertToStreamAsync(uri, file, new Format(FormatTypes.MATROSKA));

            //Assert
            Assert.True(file.Length > 0);

            Dispose();
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
            string saveFile = Guid.NewGuid().ToString() + ".mkv";

            //Act
            file = new FileStream(saveFile, FileMode.OpenOrCreate);
            var process = fFmpegClient.ConvertToPipe(uri, new Format(FormatTypes.MATROSKA));

            while (!process.HasExited)
                file.Write(await process.GetNextBytes());

            //Assert
            Assert.True(file.Length > 0);

            Dispose();
        }

        public void Dispose()
        {

            file.Close();

            if (file.GetType() == typeof(FileStream))
                File.Delete(((FileStream)file).Name);

        }
    }
}
