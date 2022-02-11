using FFmpegWrapper.Codecs;
using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Models
{
    public class CopyCodecFormat : Format, IFormat
    {

        public CopyCodecFormat(MediaTypes type, string? args = default) : base(type, args) { }
        public CopyCodecFormat(string type, string? args = default) : base(type, args) { }

        public new string GetCustomArgs() => $" -c:a copy " + Args;
    }
}
