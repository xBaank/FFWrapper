
namespace FFmpegWrapper.Models
{
    public class FFprobeClient : Client
    {
        public FFprobeClient(string ffprobePath) => Path = System.IO.Path.GetFullPath(ffprobePath);



    }
}
