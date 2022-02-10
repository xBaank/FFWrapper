using FFmpegWrapper.Codecs;
using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Models
{
    public class OpusFormat : Format, IFormat
    {
        public int Bitrate { get; set; }

        public OpusFormat(int bitrate, string? args = default) : base("Opus", args) =>
            Bitrate = bitrate;

        public new string GetCustomArgs() => $" -c:a libopus -b:a {Bitrate}K " + Args;
    }
}
