using FFmpegWrapper.Codecs;
using FFmpegWrapper.Models;

namespace FFmpegWrapper.Formats
{
    public class Format : IFormat
    {
        public string? Type { get; set; }
        public string? Args { get; set; }
        public Format() { }
        public Format(MediaTypes type, string? args = default)
        {
            Type = type.ToString();
            Args = args;
        }
        public Format(string type, string? args = default)
        {
            Type = type;
            Args = args;
        }
        public string GetFormatArg() => $"-f {Type}";
        public string GetCustomArgs() => Args ?? string.Empty;
    }
}
