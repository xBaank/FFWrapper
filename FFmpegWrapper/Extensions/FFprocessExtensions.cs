
using System.Threading.Tasks;

using FFmpegWrapper.Models;

namespace FFmpegWrapper.Extensions
{
    public static class FFprocessExtensions
    {
        public async static Task<ProcessResult> GetResultAsync(this Task<FFProcess> task)
        {
            var process = await task;
            ProcessResult processResult = new ProcessResult()
                .SetExitCode(process.ExitCode);
            return processResult;

        }

        public static ProcessResult<T> GetResult<T>(this FFProcess process, T result)
        {
            ProcessResult<T> processResult = (ProcessResult<T>)new ProcessResult<T>()
                .SetResult(result)
                .SetExitCode(process.ExitCode);
            return processResult;
        }
    }
}
