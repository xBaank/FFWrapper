using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;
using FFmpegWrapper.Extensions;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Helpers;

namespace FFmpegWrapper.Models
{
    public class FFmpegClient : Client<FFmpegClient, FFmpegProcessBuilder>
    {

        public FFmpegClient(string path) : base(path) { }
        public FFmpegClient() : base(PathUtils.TryGetFFmpegPath()) { }

        public Task<ProcessResult> ConvertToStreamAsync(string input, Stream output, IFormat outputType) => _builder
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output, outputType)
            .Build()
            .StartAsync()
            .GetResultAsync();

        public Task<ProcessResult> ConvertToStreamAsync(Stream input, IFormat inputType, Stream output, IFormat outputType) => _builder
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(output, outputType)
            .Build()
            .StartAsync()
            .GetResultAsync();

        public Task<ProcessResult> ConvertAsync(string input, string output) => _builder.CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output)
            .Build()
            .StartAsync()
            .GetResultAsync();
        public Task<ProcessResult> ConvertAsync(Stream input, string output, IFormat inputType) => _builder
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputType)
            .To(output)
            .Build()
            .StartAsync()
            .GetResultAsync();

    }

}
