
using System.Text.Json;

namespace FFmpegWrapper.Extensions
{
    internal static class JsonExtensions
    {
        internal static T ToObject<T>(this JsonElement element)
        {
            var json = element.GetRawText();
            return JsonSerializer.Deserialize<T>(json);
        }
    }
}
