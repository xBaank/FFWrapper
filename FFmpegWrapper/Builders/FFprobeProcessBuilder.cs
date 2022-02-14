using System.IO;

using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Builders
{
    public class FFprobeProcessBuilder : FFProcessBuilder<FFprobeProcessBuilder>
    {

        private Stream output = new MemoryStream();

        public FFprobeProcessBuilder From(string input) =>
            AddArguments(input);

        public FFprobeProcessBuilder From(Stream input) =>
            AddArguments("pipe:")
            .RedirectInput(true)
            .SetOutput(output)
            .SetInput(input);

        public FFprobeProcessBuilder SelectStreams(StreamType streamType, int streamNumber = 0) =>
          AddArguments($"-select_streams {streamType}:{streamNumber}");

        public FFprobeProcessBuilder ShowPackets() =>
          AddArguments($"-show_packets");

        public FFprobeProcessBuilder ShowFrames() =>
          AddArguments($"-show_frames");

        public FFprobeProcessBuilder Reconnect() =>
           AddArguments("-reconnect 1");

        public FFprobeProcessBuilder ShowFormat() =>
            AddArguments("-show_format");

        public FFprobeProcessBuilder AsJson() =>
            AddArguments("-of json");

    }
}
