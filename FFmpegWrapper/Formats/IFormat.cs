
namespace FFmpegWrapper.Formats
{
    public interface IFormat
    {
        public string GetFormatArg();
        public string GetCustomArgs();
    }
}
