
using WebmOpus.Models;

namespace WebmOpus.Extensions
{
    public static class MediaTypesExtensions
    {
        public static string GetArgs(this MediaTypes mediaTypes) => mediaTypes switch
        {
            MediaTypes.MP4 => $"mp4 -movflags empty_moov",
            _ => mediaTypes.ToString()
        };

    }
}
