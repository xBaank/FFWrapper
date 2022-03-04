using System;
using System.IO;

using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFmpegAudioTests
    {
        private FFmpegClient fFmpegClient = new FFmpegClient();
        Stream stream;



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
            stream = File.Open(Path.Combine(Directory.GetCurrentDirectory(), saveFile), FileMode.Open);

            //Assert
            Assert.True(stream.Length > 0);

            stream.Dispose();
        }

        [Theory]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public async void VideoShouldConvertToStream(string uri)
        {

            //Act
            stream = new MemoryStream();
            await fFmpegClient.ConvertToStreamAsync(uri, stream, new Format(FormatTypes.OPUS));

            //Assert
            Assert.True(stream.Length > 0);

            stream.Dispose();
        }

        [Fact]
        public async void VideoConvertShouldThrow()
        {
            stream = new MemoryStream();

            await Assert.ThrowsAsync<NullReferenceException>(async () => { await fFmpegClient.ConvertToStreamAsync(null, new Format(FormatTypes.MP4), stream, new Format(FormatTypes.MATROSKA)); });

            stream.Dispose();
        }
    }
}
