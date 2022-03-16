using System;
using System.IO;

using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Builders
{
    public class FFmpegProcessBuilder : FFProcessBuilder<FFmpegProcessBuilder>
    {

        public FFmpegProcessBuilder To(Action<IFormat> options)
        {
            IFormat format = new Format();
            options(format);

            return RedirectOutput(true)
            .AddArguments(format.MediaFormat)
            .AddArguments(format.Args)
            .AddArguments("pipe:");
        }

        public FFmpegProcessBuilder To(Stream output, Action<IFormat> options) =>
            To(options)
            .SetOutput(output);

        public override FFmpegProcessBuilder To(string output) =>
            RedirectOutput(false)
            .AddArguments(output);

        public FFmpegProcessBuilder From(Action<IFormat> options)
        {
            IFormat format = new Format();
            options(format);

            return AddArguments(format.MediaFormat)
            .AddArguments(format.Args)
            .AddArguments("-i pipe:")
            .RedirectInput(true);
        }

        public FFmpegProcessBuilder From(Stream input, Action<IFormat> options) =>
            From(options)
            .SetInput(input);

        public override FFmpegProcessBuilder From(string input) =>
            AddArguments($"-i {input}")
            .RedirectInput(false);
    }
}

