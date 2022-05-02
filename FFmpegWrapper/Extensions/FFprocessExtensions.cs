
using System.Threading.Tasks;

using FFmpegWrapper.Models;

namespace FFmpegWrapper.Extensions
{
    internal static class FFprocessExtensions
    {
        internal static async Task<ProcessResult> GetResultAsync(this Task<FFProcess> task)
        {
            var process = await task;

            await process.KillProcess();

            var processResult = new ProcessResult()
                .SetExitCode(process.ExitCode);
            return processResult;

        }
        internal static async Task<ProcessResult<T>> GetResultAsync<T>(this Task<FFProcess> task, string property)
        {
            var process = await task;

            await process.KillProcess();

            var result = await process.DeserializeResultAsync<T>(property);

            var processResult = (ProcessResult<T>)new ProcessResult<T>()
               .SetResult(result)
               .SetExitCode(process.ExitCode);

            return processResult;

        }
    }
}
