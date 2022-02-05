using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FFmpegWrapper.Extensions
{
    public static class IEnumerableExtensions
    {
        public static Task WhenAll(this IEnumerable<Task> tasks) => Task.WhenAll(tasks);
        public static void WaitAll(this IEnumerable<Task> tasks) => Task.WaitAll(tasks.ToArray());
        
    }
}
