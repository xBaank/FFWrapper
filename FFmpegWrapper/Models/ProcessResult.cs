
namespace FFmpegWrapper.Models
{
    public class ProcessResult<T> : ProcessResult
    {
        public T? Result { get; set; } = default;
        public ProcessResult SetResult(T? result)
        {
            Result = result;
            return this;
        }
    }

    public class ProcessResult
    {
        public int ExitCode { get; set; }
        public ProcessResult SetExitCode(int code)
        {
            ExitCode = code;
            return this;
        }


    }

}
