
using System;
using System.Threading.Tasks;

namespace FFmpegWrapper.Extensions
{
    internal static class TaskExtension
    {
        internal static async Task NotThrow(this Task task)
        {
            try
            {
                await task;
            }
            catch (OperationCanceledException) { }
        }

        internal static async ValueTask<T?> NotThrow<T>(this ValueTask<T?> task)
        {
            try
            {
                return await task;
            }
            catch (OperationCanceledException)
            {
                return default;
            }
        }
    }
}
