using System.IO;
using System.Net.Http;

using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFprobeTests
    {
        private FFprobeClient ffProbeClient = new FFprobeClient();


        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(AudioFilesUri.OGG)]
        public async void FileShouldGetFormat(string uri)
        {
            var metadata = await ffProbeClient.GetMetadataAsync(uri);

            Assert.NotNull(metadata.Result);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(AudioFilesUri.OGG)]
        public async void FileShouldGetFormatFromStream(string uri)
        {
            var stream = await new HttpClient().GetStreamAsync(uri);

            var metadata = await ffProbeClient.GetMetadataAsync(stream);

            Assert.NotNull(metadata);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(AudioFilesUri.OGG, StreamType.a)]
        public async void FileShouldGetPacketsAndFrames(string uri, StreamType streamType)
        {
            var packets = await ffProbeClient.GetPacketsAsync(uri, streamType);
            var frames = await ffProbeClient.GetFramesAsync(uri, streamType);

            Assert.True(packets?.Result.Count > 0);
            Assert.True(frames?.Result.Count > 0);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(VideoFilesUri.WEBM, StreamType.v)]
        public async void FileShouldGetPacketsAndFramesFromStream(string uri, StreamType streamType)
        {
            var stream = await new HttpClient().GetByteArrayAsync(uri);

            var packets = await ffProbeClient.GetPacketsAsync(new MemoryStream(stream), streamType);
            var frames = await ffProbeClient.GetFramesAsync(new MemoryStream(stream), streamType);

            Assert.True(packets?.Result.Count > 0);
            Assert.True(frames?.Result.Count > 0);
        }
    }
}
