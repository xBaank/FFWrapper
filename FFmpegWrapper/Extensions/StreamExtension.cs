
using System;
using System.Threading.Tasks;

namespace FFmpegWrapper.Extensions
{
    public static class TaskExtension
    {
        public static async Task NotThrow(this Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException) { }
        }

        public static async ValueTask<T?> NotThrow<T>(this ValueTask<T?> task)
        {
            try
            {
                return await task;
            }
            catch (OperationCanceledException)
            {
                return default(T);
            }
        }
    }
}
