using FFmpegWrapper.Formats;
using FFmpegWrapper.Models;
using FFmpegWrapper.Tests.Data;

using System.IO;
using System.Net.Http;

using Xunit;

namespace FFmpegWrapper.Tests
{
    public class FFprobeTests
    {
        private FFprobeClient ffProbeClient = new FFprobeClient();
        HttpClient httpClient = new HttpClient();


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
        public async void FileShouldGetFormatFromUri(string uri)
        {
            var metadata = await ffProbeClient.GetMetadataAsync(uri);

            Assert.NotNull(metadata);
        }

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
        public async void FileShouldGetFormatFromUriToStream(string uri)
        {
            MemoryStream ms = new MemoryStream();
            var metadata = await ffProbeClient.GetMetadataAsync(uri, ms);

            Assert.True(ms.Length > 0 && metadata is not null);
        }

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
        public async void FileShouldGetFormatFromStream(string uri)
        {
            var stream = await httpClient.GetStreamAsync(uri);
            var metadata = await ffProbeClient.GetMetadataAsync(stream);

            Assert.NotNull(metadata);
        }

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
        public async void FileShouldGetFormatFromStreamToStream(string uri)
        {
            MemoryStream ms = new MemoryStream();
            var stream = await httpClient.GetStreamAsync(uri);

            var metadata = await ffProbeClient.GetMetadataAsync(stream, ms);
            Assert.True(ms.Length > 0 && metadata is not null);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(VideoFilesUri.MOV, StreamType.v)]
        [InlineData(VideoFilesUri.OGG, StreamType.v)]
        [InlineData(VideoFilesUri.MP4, StreamType.v)]
        [InlineData(VideoFilesUri.AVI, StreamType.v)]
        [InlineData(VideoFilesUri.WEBM, StreamType.v)]
        [InlineData(AudioFilesUri.WAV, StreamType.a)]
        [InlineData(AudioFilesUri.MP3, StreamType.a)]
        [InlineData(AudioFilesUri.OGG, StreamType.a)]
        public async void FileShouldGetPacketsAndFramesFromUri(string uri, StreamType streamType)
        {
            var packets = await ffProbeClient.GetPacketsAsync(uri, streamType);
            var frames = await ffProbeClient.GetFramesAsync(uri, streamType);

            Assert.True(packets?.Count > 0);
            Assert.True(frames?.Count > 0);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(VideoFilesUri.MOV, StreamType.v)]
        [InlineData(VideoFilesUri.OGG, StreamType.v)]
        [InlineData(VideoFilesUri.MP4, StreamType.v)]
        [InlineData(VideoFilesUri.AVI, StreamType.v)]
        [InlineData(VideoFilesUri.WEBM, StreamType.v)]
        [InlineData(AudioFilesUri.WAV, StreamType.a)]
        [InlineData(AudioFilesUri.MP3, StreamType.a)]
        [InlineData(AudioFilesUri.OGG, StreamType.a)]
        public async void FileShouldGetPacketsAndFramesFromUriToStream(string uri, StreamType streamType)
        {
            var ms = new MemoryStream();

            var packets = await ffProbeClient.GetPacketsAsync(uri, streamType, ms);
            Assert.True(ms.Length > 0 && packets?.Count > 0);

            ms = new MemoryStream();

            var frames = await ffProbeClient.GetFramesAsync(uri, streamType, ms);
            Assert.True(ms.Length > 0 && frames?.Count > 0);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(VideoFilesUri.MOV, StreamType.v)]
        [InlineData(VideoFilesUri.OGG, StreamType.v)]
        [InlineData(VideoFilesUri.MP4, StreamType.v)]
        [InlineData(VideoFilesUri.AVI, StreamType.v)]
        [InlineData(VideoFilesUri.WEBM, StreamType.v)]
        [InlineData(AudioFilesUri.WAV, StreamType.a)]
        [InlineData(AudioFilesUri.MP3, StreamType.a)]
        [InlineData(AudioFilesUri.OGG, StreamType.a)]
        public async void FileShouldGetPacketsAndFramesFromStream(string uri, StreamType streamType)
        {
            var bytes = await httpClient.GetByteArrayAsync(uri);

            var packets = await ffProbeClient.GetPacketsAsync(new MemoryStream(bytes), streamType);
            var frames = await ffProbeClient.GetFramesAsync(new MemoryStream(bytes), streamType);

            Assert.True(packets?.Count > 0);
            Assert.True(frames?.Count > 0);
        }

        [Theory]
        [InlineData(VideoFilesUri.WMV, StreamType.v)]
        [InlineData(VideoFilesUri.MOV, StreamType.v)]
        [InlineData(VideoFilesUri.OGG, StreamType.v)]
        [InlineData(VideoFilesUri.MP4, StreamType.v)]
        [InlineData(VideoFilesUri.AVI, StreamType.v)]
        [InlineData(VideoFilesUri.WEBM, StreamType.v)]
        [InlineData(AudioFilesUri.WAV, StreamType.a)]
        [InlineData(AudioFilesUri.MP3, StreamType.a)]
        [InlineData(AudioFilesUri.OGG, StreamType.a)]
        public async void FileShouldGetPacketsAndFramesFromStreamToStream(string uri, StreamType streamType)
        {
            var bytes = await httpClient.GetByteArrayAsync(uri);
            var ms = new MemoryStream();

            var packets = await ffProbeClient.GetPacketsAsync(new MemoryStream(bytes), streamType, ms);
            Assert.True(ms.Length > 0 && packets?.Count > 0);

            ms = new MemoryStream();
            var frames = await ffProbeClient.GetFramesAsync(new MemoryStream(bytes), streamType, ms);

            Assert.True(ms.Length > 0 && frames?.Count > 0);
        }
    }
}
