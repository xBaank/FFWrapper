using System;
using System.IO;

using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFmpegAudioTests : IDisposable
    {
        private FFmpegClient fFmpegClient = new FFmpegClient(Directory.GetCurrentDirectory() + "/FFMPEG/ffmpeg.exe");
        private Stream file;

        [Theory]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public async void VideoShouldConvertToOpus(string uri)
        {
            //Arrange
            string saveFile = Guid.NewGuid().ToString() + ".Opus";

            //Act
            await fFmpegClient.ConvertAsync(uri, saveFile);
            file = File.Open(Path.Combine(Directory.GetCurrentDirectory(), saveFile), FileMode.Open);

            //Assert
            Assert.True(file.Length > 0);
        }

        [Theory]
        [InlineData(AudioFilesUri.WAV)]
        [InlineData(AudioFilesUri.MP3)]
        [InlineData(AudioFilesUri.OGG)]
        public async void VideoShouldConvertToStream(string uri)
        {
            //Arrange
            string saveFile = Guid.NewGuid().ToString() + ".opus";

            //Act
            file = new FileStream(saveFile, FileMode.OpenOrCreate);
            await fFmpegClient.ConvertToStreamAsync(uri, file, new Format(MediaTypes.OPUS));

            //Assert
            Assert.True(file.Length > 0);

            Dispose();
        }

        public void Dispose()
        {
            if (file is null)
                return;

            file.Close();

            if (file.GetType() == typeof(FileStream))
                File.Delete(((FileStream)file).Name);

        }
    }
}
