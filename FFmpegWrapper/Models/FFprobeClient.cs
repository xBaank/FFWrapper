using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;
using FFmpegWrapper.Formats;
using FFmpegWrapper.JsonModels;

namespace FFmpegWrapper.Models
{
    public class FFprobeClient : Client
    {
        public FFprobeClient(string path) : base(path) { }

        public async Task<FormatMetadata?> GetMetadataAsync(string input)
        {
            var process = MetadataProcess(input);
            await process.StartAsync();
            return JsonSerializer.Deserialize<FormatRoot>(await process.ReadAsStringAsync())?.Format;
        }

        public FormatMetadata? GetMetadata(string input) =>
            GetMetadataAsync(input)
            .GetAwaiter()
            .GetResult();

        public async Task<List<Packet>?> GetPacketsAsync(string input, StreamType streamType, int timeStart = 0, int timeAdded = 0, int streamNumber = 0)
        {
            var process = PacketsProcess(input, streamType, timeStart, timeAdded, streamNumber);
            await process.StartAsync();
            return JsonSerializer.Deserialize<PacketRoot>(await process.ReadAsStringAsync())?.PacketsData;
        }

        public List<Packet>? GetPackets(string input, StreamType streamType, int timeStart = 0, int timeAdded = 0, int streamNumber = 0) =>
            GetPacketsAsync(input, streamType, timeStart, timeAdded, streamNumber)
            .GetAwaiter()
            .GetResult();

        public async Task<List<Frame>?> GetFramesAsync(string input, StreamType streamType, int timeStart = 0, int timeAdded = 0, int streamNumber = 0)
        {
            var process = FramesProcess(input, streamType, timeStart, timeAdded, streamNumber);
            await process.StartAsync();
            return JsonSerializer.Deserialize<FrameRoot>(await process.ReadAsStringAsync())?.Frames;
        }

        public List<Frame>? GetFrames(string input, StreamType streamType, int timeStart = 0, int timeAdded = 0, int streamNumber = 0) =>
            GetFramesAsync(input, streamType, timeStart, timeAdded, streamNumber)
            .GetAwaiter()
            .GetResult();

        private FFprobeProcessBuilder CreateFFProbeBuilder() => new FFprobeProcessBuilder()
            .ShellExecute(false)
            .CreateNoWindow(true)
            .Path(Path);

        private FFProcess MetadataProcess(string input) => CreateFFProbeBuilder()
            .RedirectOutput(true)
            .SetOutput(new MemoryStream())
            .ShowFormat()
            .Reconnect()
            .From(input)
            .AsJson()
            .Build();

        private FFProcess PacketsProcess(string input, StreamType streamType, int timeStart = 0, int timeAdded = 0, int streamNumber = 0) => CreateFFProbeBuilder()
            .RedirectOutput(true)
            .SetOutput(new MemoryStream())
            .From(input)
            .SelectStreams(streamType, streamNumber)
            .ShowPackets()
            .ReadIntervals()
            .WithInterval(timeStart, timeAdded)
            .Reconnect()
            .AsJson()
            .Build();

        private FFProcess FramesProcess(string input, StreamType streamType, int timeStart = 0, int timeAdded = 0, int streamNumber = 0) => CreateFFProbeBuilder()
            .RedirectOutput(true)
            .SetOutput(new MemoryStream())
            .From(input)
            .SelectStreams(streamType, streamNumber)
            .ShowFrames()
            .ReadIntervals()
            .WithInterval(timeStart, timeAdded)
            .Reconnect()
            .AsJson()
            .Build();

    }
}
