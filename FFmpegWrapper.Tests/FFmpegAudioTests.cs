using System;
using System.IO;
using System.Net.Http;

using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFmpegAudioTests
    {
        private FFmpegClient fFmpegClient = new FFmpegClient();
        Stream file;

        HttpClient httpClient = new HttpClient();


        [Theory]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public async void VideoShouldConvertToFile(string uri)
        {
            //Arrange
            string saveFile = Guid.NewGuid().ToString() + ".Opus";

            //Act
            await fFmpegClient.ConvertAsync(uri, saveFile);
            file = File.Open(Path.Combine(Directory.GetCurrentDirectory(), saveFile), FileMode.Open);

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }

        [Theory]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public async void VideoShouldConvertToStream(string uri)
        {

            //Act
            file = new MemoryStream();
            await fFmpegClient.ConvertToStreamAsync(uri, file, new Format(FormatTypes.OPUS));

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }

        [Theory]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public async void VideoShouldConvertToPipe(string uri)
        {
            //Arrange

            //Act
            file = new MemoryStream();
            var process = fFmpegClient.ConvertToPipe(uri, new Format(FormatTypes.OPUS));

            byte[] bytesread = new byte[0];
            while (!process.HasExited || bytesread.Length > 0)
                await file.WriteAsync(bytesread = await process.GetNextBytes());

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }

        [Theory]
        [InlineData(AudioFilesUri.WAV, FormatTypes.WAV)]
        [InlineData(AudioFilesUri.MP3, FormatTypes.MP3)]
        [InlineData(AudioFilesUri.OGG, FormatTypes.OGG)]
        public async void VideoShouldConvertToPipeFromStream(string uri, FormatTypes formatType)
        {
            //Arrange

            //Act
            file = new MemoryStream();
            var process = fFmpegClient.ConvertToPipe(await httpClient.GetStreamAsync(uri), new Format(formatType), new Format(FormatTypes.OPUS));

            byte[] bytesread = new byte[0];
            while (!process.HasExited || bytesread.Length > 0)
                await file.WriteAsync(bytesread = await process.GetNextBytes());

            //Assert
            Assert.True(file.Length > 0);

            file.Dispose();
        }
    }
}
