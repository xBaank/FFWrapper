using FFmpegWrapper.Codecs;
using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Models
{
    public class PcmFormat : Format, IFormat
    {
        public PCMFormatsTypes PcmFormatType { get; set; }
        public int Fs { get; set; }
        public int Channels { get; set; }

        public PcmFormat(PCMFormatsTypes pcmFormat, int fs, int channels = 2, string? args = default) : base(pcmFormat.ToString(), args)
        {
            PcmFormatType = pcmFormat;
            Fs = fs;
            Channels = channels;
        }

        public new string GetCustomArgs() => $" -ac {Channels} -ar {Fs} " + Args;
    }
}
