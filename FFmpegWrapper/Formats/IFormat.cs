
namespace FFmpegWrapper.Formats
{
    public interface IFormat
    {
        public string MediaFormat { get; }
        public string Args { get; }
        public IFormat WithFormat(FormatTypes format);
        public IFormat WithFormat(string format);
        public IFormat WithArgs(string args);
    }
}
