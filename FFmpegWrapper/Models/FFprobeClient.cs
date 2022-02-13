using System.Text.Json;

using FFmpegWrapper.Builders;
using FFmpegWrapper.JsonModels;

namespace FFmpegWrapper.Models
{
    public class FFprobeClient : Client
    {
        public FFprobeClient(string ffprobePath) => Path = System.IO.Path.GetFullPath(ffprobePath);

        public FormatMetadata GetMetadata(string input)
        {
            var process = MetadataProcess(input);
            process.Start();
            return JsonSerializer.Deserialize<Root>(process.Output)?.Format
                ?? throw new System.Exception("No se ha podido deserializar");
        }

        private FFprobeProcessBuilder CreateFFProbeBuilder() => new FFprobeProcessBuilder()
            .ShellExecute(false)
            .CreateNoWindow(true)
            .Path(Path);

        private FFProbeProcess MetadataProcess(string input) => CreateFFProbeBuilder()
            .RedirectOutput(true)
            .ShowFormat()
            .Reconnect()
            .From(input)
            .AsJson()
            .Build();

    }
}
