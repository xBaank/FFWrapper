using System;
using System.IO;
using System.Threading.Tasks;

using FFmpegWrapper.Builders;
using FFmpegWrapper.Extensions;
using FFmpegWrapper.Formats;
using FFmpegWrapper.Utils;

namespace FFmpegWrapper.Models
{
    public class FFmpegClient : Client<FFmpegClient, FFmpegProcessBuilder>
    {

        public FFmpegClient(string path) : base(path) { }
        public FFmpegClient() : base(PathUtils.TryGetFFmpegPath()) { }

        public Task<ProcessResult> ConvertToStreamAsync(string input, Stream output, Action<IFormat> options) => new FFmpegProcessBuilder()
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output, options)
            .Build()
            .StartAsync()
            .GetResultAsync();
            
        public Task<ProcessResult> ConvertToStreamAsync(Stream input, Action<IFormat> inputOptions, Stream output, Action<IFormat> outputOptions) => new FFmpegProcessBuilder()
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, inputOptions)
            .To(output, outputOptions)
            .Build()
            .StartAsync()
            .GetResultAsync();

        public Task<ProcessResult> ConvertAsync(string input, string output) => new FFmpegProcessBuilder()
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input)
            .To(output)
            .Build()
            .StartAsync()
            .GetResultAsync();

        public Task<ProcessResult> ConvertAsync(Stream input, string output, Action<IFormat> options) => new FFmpegProcessBuilder()
            .CreateFFBuilder(Path)
            .RaiseErrorEvents(ErrorRecieved)
            .RaiseExitErrorEvent(ExitWithErrorRecieved)
            .From(input, options)
            .To(output)
            .Build()
            .StartAsync()
            .GetResultAsync();

    }

}
