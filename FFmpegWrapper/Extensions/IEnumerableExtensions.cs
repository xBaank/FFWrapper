using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFmpegWrapper.Extensions
{
    internal static class IEnumerableExtensions
    {
        internal static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

    }
}
