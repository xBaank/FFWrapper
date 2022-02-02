
using WebmOpus.Models;

namespace WebmOpus.Extensions
{
    internal static class MediaTypesExtensions
    {
        internal static string GetArgs(this MediaTypes mediaTypes) => mediaTypes switch
        {
            MediaTypes.MP4 => $"mp4 -movflags empty_moov",
            _ => mediaTypes.ToString()
        };

    }
}
