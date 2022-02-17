using System.IO;

using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Builders
{
    public class FFmpegProcessBuilder : FFProcessBuilder<FFmpegProcessBuilder>
    {

        public FFmpegProcessBuilder To(IFormat format) =>
            RedirectOutput(true)
            .AddArguments(format.GetFormatArg())
            .AddArguments(format.GetCustomArgs())
            .AddArguments("pipe:");

        public FFmpegProcessBuilder To(Stream output, IFormat format) =>
            To(format)
            .SetOutput(output);

        public override FFmpegProcessBuilder To(string output) =>
            RedirectOutput(false)
            .AddArguments(output);

        public FFmpegProcessBuilder From(IFormat type) =>
            AddArguments(type.GetFormatArg())
            .AddArguments(type.GetCustomArgs())
            .AddArguments("-i pipe:")
            .RedirectInput(true);

        public FFmpegProcessBuilder From(Stream input, IFormat format) =>
            From(format)
            .SetInput(input);

        public override FFmpegProcessBuilder From(string input) =>
            AddArguments($"-i {input}")
            .RedirectInput(false);
    }
}

