using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;
using FFmpegWrapper.JsonModels;

namespace FFmpegWrapper.Models
{
    public class FFprobeClient : Client
    {
        public FFprobeClient(string path) : base(path) { }

        public async Task<FormatMetadata?> GetMetadataAsync(string input)
        {
            var process = MetadataProcess(input);
            await process.StartAsync();
            return JsonSerializer.Deserialize<Root>(await process.ReadAsStringAsync())?.Format;
        }

        public FormatMetadata? GetMetadata(string input) => GetMetadataAsync(input).GetAwaiter().GetResult();

        private FFprobeProcessBuilder CreateFFProbeBuilder() => new FFprobeProcessBuilder()
            .ShellExecute(false)
            .CreateNoWindow(true)
            .Path(Path);

        private FFProcess MetadataProcess(string input) => CreateFFProbeBuilder()
            .RedirectOutput(true)
            .SetOutput(new MemoryStream())
            .ShowFormat()
            .Reconnect()
            .From(input)
            .AsJson()
            .Build();

    }
}
