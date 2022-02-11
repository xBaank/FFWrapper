using FFmpegWrapper.Codecs;
using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Models
{
    public class OpusFormat : Format, IFormat
    {
        public int Bitrate { get; set; }

        public OpusFormat(string type = "Opus", int bitrate = 96, string? args = default) : base(type, args) =>
            Bitrate = bitrate;

        public new string GetCustomArgs() => $" -c:a libopus -b:a {Bitrate}K " + Args;
    }
}
