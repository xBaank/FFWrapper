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
        public async void FileShouldGetFormatFromUri(string uri)
        {
            var metadata = await ffProbeClient.GetMetadataAsync(uri);

            Assert.NotNull(metadata);
        }

        //[Theory]
        //[InlineData(VideoFilesUri.WMV)]
        //[InlineData(AudioFilesUri.OGG)]
        //public async void FileShouldGetFormatFromStream(string uri)
        //{
        //    var stream = await httpClient.GetStreamAsync(uri);
        //    var metadata = await ffProbeClient.GetMetadataAsync(stream);

        //    Assert.NotNull(metadata);
        //}

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


        //[Theory]
        //[InlineData(VideoFilesUri.WMV, StreamType.v)]
        //[InlineData(VideoFilesUri.WEBM, StreamType.v)]
        //public async void FileShouldGetPacketsAndFramesFromStream(string uri, StreamType streamType)
        //{
        //    var bytes = await httpClient.GetStreamAsync(uri);

        //    var packets = await ffProbeClient.GetPacketsAsync(bytes, streamType);

        //    bytes = await httpClient.GetStreamAsync(uri);
        //    var frames = await ffProbeClient.GetFramesAsync(bytes, streamType);

        //    Assert.True(packets?.Count > 0);
        //    Assert.True(frames?.Count > 0);
        //}
    }
}
