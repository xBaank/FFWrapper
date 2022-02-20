using System.Collections.Generic;
using System.Threading.Tasks;

namespace FFmpegWrapper.Extensions
{
    public static class IEnumerableExtensions
    {
        public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);

    }
}
