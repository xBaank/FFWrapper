
namespace FFmpegWrapper.Formats
{
    public class CopyCodecFormat : Format, IFormat
    {
        public CocecFormatTypes CocecFormat { get; set; }
        public CopyCodecFormat(MediaTypes type, CocecFormatTypes cocecFormat, string? args = default) : base(type, args) =>
            CocecFormat = cocecFormat;
        public CopyCodecFormat(string type, CocecFormatTypes cocecFormat, string? args = default) : base(type, args) =>
            CocecFormat = cocecFormat;

        public new string GetCustomArgs() => ($" -c:{CocecFormat.ToString().ToLowerInvariant()} copy " + Args);
    }
    public enum CocecFormatTypes
    {
        /// <summary>
        /// Audio
        /// </summary>
        A,
        /// <summary>
        /// Video
        /// </summary>
        V,
        /// <summary>
        /// Subtitles
        /// </summary>
        S
    }
}
