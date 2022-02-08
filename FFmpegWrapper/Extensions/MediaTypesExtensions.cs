
using FFmpegWrapper.Models;

namespace FFmpegWrapper.Extensions
{
    internal static class MediaTypesExtensions
    {
        internal static string GetOutPutArgs(this MediaTypes mediaTypes) => mediaTypes switch
        {
            MediaTypes.MP4 => $"-f {mediaTypes} -movflags empty_moov pipe:",
            MediaTypes.OPUS => $"-map 0 -f data -c:a libopus -b:a 96K pipe:",
            _ => $"-f {mediaTypes} pipe:"
        };

        internal static string GetInputArgs(this MediaTypes mediaTypes) => mediaTypes switch
        {
            _ => $"-f {mediaTypes} -i pipe:"
        };

    }
}
