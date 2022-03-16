
namespace FFmpegWrapper.Formats
{
    public class Format : IFormat
    {
        private string? _mediaFormat;
        private string? _args;
        public string MediaFormat
        {
            get => $" -f {_mediaFormat} ";
            set => _mediaFormat = value;
        }
        public string Args
        {
            get => _args ?? string.Empty;
            set => _args = value;
        }

        public IFormat WithFormat(FormatTypes format)
        {
            MediaFormat = format.ToString();
            return this;
        }
        public IFormat WithFormat(string format)
        {
            MediaFormat = format;
            return this;
        }

        public IFormat WithArgs(string args)
        {
            Args = args;
            return this;
        }
    }
}
