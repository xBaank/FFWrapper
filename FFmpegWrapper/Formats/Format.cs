
namespace FFmpegWrapper.Formats
{
    public class Format : IFormat
    {
        public string? MediaFormat { get; set; }
        public string? Args { get; set; }
        public Format() { }
        public Format(FormatTypes format, string? args = default)
        {
            MediaFormat = format.ToString();
            Args = args;
        }
        public Format(string format, string? args = default)
        {
            MediaFormat = format;
            Args = args;
        }
        public string GetFormatArg() => $" -f {MediaFormat} ";
        public string GetCustomArgs() => Args ?? string.Empty;
    }
}
