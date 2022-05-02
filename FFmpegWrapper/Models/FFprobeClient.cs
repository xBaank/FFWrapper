using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;
using FFmpegWrapper.Formats;
using FFmpegWrapper.JsonModels;
using FFmpegWrapper.Extensions;
using FFmpegWrapper.Utils;

namespace FFmpegWrapper.Models
{
    public class FFprobeClient : Client<FFprobeClient, FFprobeProcessBuilder>
    {
        public FFprobeClient(string path) : base(path) { }
        public FFprobeClient() : base(PathUtils.TryGetFFprobePath()) { }

        public async Task<ProcessResult<FormatMetadata?>> GetMetadataAsync(string input, Stream output) =>
            await GetMetadataDeserializeAsync(input, output);

        public async Task<ProcessResult<FormatMetadata?>> GetMetadataAsync(Stream input, Stream output) =>
           await GetMetadataDeserializeAsync(input, output);

        public async Task<ProcessResult<FormatMetadata?>> GetMetadataAsync(string input) =>
            await GetMetadataDeserializeAsync(input, new MemoryStream());

        public async Task<ProcessResult<FormatMetadata?>> GetMetadataAsync(Stream input) =>
           await GetMetadataDeserializeAsync(input, new MemoryStream());

        public async Task<ProcessResult<List<Packet>?>> GetPacketsAsync(string input, StreamType streamType, Stream output, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetPacketsDeserializeAsync(input, streamType, output, timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Packet>?>> GetPacketsAsync(Stream input, StreamType streamType, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetPacketsDeserializeAsync(input, streamType, new MemoryStream(), timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Packet>?>> GetPacketsAsync(string input, StreamType streamType, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetPacketsDeserializeAsync(input, streamType, new MemoryStream(), timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Packet>?>> GetPacketsAsync(Stream input, StreamType streamType, Stream output, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetPacketsDeserializeAsync(input, streamType, output, timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Frame>?>> GetFramesAsync(string input, StreamType streamType, Stream output, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetFramesDeserializeAsync(input, streamType, output, timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Frame>?>> GetFramesAsync(Stream input, StreamType streamType, Stream output, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetFramesDeserializeAsync(input, streamType, output, timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Frame>?>> GetFramesAsync(string input, StreamType streamType, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetFramesDeserializeAsync(input, streamType, new MemoryStream(), timeStart, timeAdded, streamNumber);

        public async Task<ProcessResult<List<Frame>?>> GetFramesAsync(Stream input, StreamType streamType, double timeStart = 0, double timeAdded = 0, int streamNumber = 0) =>
            await GetFramesDeserializeAsync(input, streamType, new MemoryStream(), timeStart, timeAdded, streamNumber);

        private async Task<ProcessResult<List<Frame>?>> GetFramesDeserializeAsync(dynamic input, StreamType streamType, dynamic output, double timeStart, double timeAdded, int streamNumber)
        {
            FFProcess process = FramesProcessBuild(input, streamType, output, timeStart, timeAdded, streamNumber);
            return await process.StartAsync().GetResultAsync<List<Frame>?>("frames");
        }

        private async Task<ProcessResult<List<Packet>?>> GetPacketsDeserializeAsync(dynamic input, StreamType streamType, dynamic output, double timeStart = 0, double timeAdded = 0, int streamNumber = 0)
        {
            FFProcess process = PacketsProcessBuild(input, streamType, output, timeStart, timeAdded, streamNumber);
            return await process.StartAsync().GetResultAsync<List<Packet>?>("packets");
        }

        private async Task<ProcessResult<FormatMetadata?>> GetMetadataDeserializeAsync(dynamic input, dynamic output)
        {
            FFProcess process = MetadataProcessBuild(input, output);
            return await process.StartAsync().GetResultAsync<FormatMetadata?>("format");
        }

        private FFProcess MetadataProcessBuild(dynamic input, dynamic output) => MetadataProcessBuilder()
            .From(input)
            .To(output)
            .Build();

        private FFprobeProcessBuilder MetadataProcessBuilder() => new FFprobeProcessBuilder()
           .CreateFFBuilder(Path)
           .ShowFormat()
           .Reconnect()
           .AsJson();

        private FFProcess PacketsProcessBuild(dynamic input, StreamType streamType, dynamic output, double timeStart, double timeAdded, int streamNumber) =>
            PacketsProcessBuilder(streamType, timeStart, timeAdded, streamNumber)
            .From(input)
            .To(output)
            .Build();

        private FFprobeProcessBuilder PacketsProcessBuilder(StreamType streamType, double timeStart, double timeAdded, int streamNumber) => new FFprobeProcessBuilder()
            .CreateFFBuilder(Path)
            .SelectStreams(streamType, streamNumber)
            .ShowPackets()
            .ReadIntervals()
            .WithInterval(timeStart, timeAdded)
            .Reconnect()
            .AsJson();

        private FFProcess FramesProcessBuild(dynamic input, StreamType streamType, dynamic output, double timeStart, double timeAdded, int streamNumber) =>
            FramesProcessBuilder(streamType, timeStart, timeAdded, streamNumber)
            .From(input)
            .To(output)
            .Build();

        private FFprobeProcessBuilder FramesProcessBuilder(StreamType streamType, double timeStart, double timeAdded, int streamNumber) => new FFprobeProcessBuilder()
            .CreateFFBuilder(Path)
            .SelectStreams(streamType, streamNumber)
            .ShowFrames()
            .ReadIntervals()
            .WithInterval(timeStart, timeAdded)
            .Reconnect()
            .AsJson();

    }
}
