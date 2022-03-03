using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using Xunit;

namespace FFmpegWrapper.Tests
{
    [CollectionDefinition(nameof(FFprobeTests), DisableParallelization = true)]
    public class FFprobeTests
    {
        private FFprobeClient ffProbeClient = new FFprobeClient();


        [Theory]
        [InlineData(VideoFilesUri.WMV)]
        [InlineData(AudioFilesUri.OGG)]
        public async void FileShouldGetFormatFromUri(string uri)
        {
            var metadata = await ffProbeClient.GetMetadataAsync(uri);

            Assert.NotNull(metadata);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(AudioFilesUri.OGG, StreamType.a)]
        public async void FileShouldGetPacketsAndFramesFromUri(string uri, StreamType streamType)
        {
            var packets = await ffProbeClient.GetPacketsAsync(uri, streamType);
            var frames = await ffProbeClient.GetFramesAsync(uri, streamType);

            Assert.True(packets?.Count > 0);
            Assert.True(frames?.Count > 0);
        }
    }
}
