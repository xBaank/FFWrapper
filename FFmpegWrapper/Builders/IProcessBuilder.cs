
namespace FFmpegWrapper.Builders
{
    ///<summary>Interface for a ProcessBuilder</summary>
    /// <typeparam name="T">Implementation type</typeparam>
    /// <typeparam name="K">Build type</typeparam>
    public interface IProcessBuilder<T, K>
    {
        public K Build();
        public T ShellExecute(bool value);
        public T CreateNoWindow(bool value);
        public T Path(string value);
        public T RedirectOutput(bool value);
        public T RedirectInput(bool value);
        public T RedirectError(bool value);
        public T AddArguments(string args);
        public T SetArguments(string args);
    }
}
