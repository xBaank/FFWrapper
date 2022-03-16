using System;
using System.IO;

using FFmpegWrapper.Formats;

namespace FFmpegWrapper.Builders
{
    public class FFmpegProcessBuilder : FFProcessBuilder<FFmpegProcessBuilder>
    {

        public FFmpegProcessBuilder To(Action<IFormat> options)
        {
            var format = GetFormat(options);

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
            var format = GetFormat(options);

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

        private IFormat GetFormat(Action<IFormat> options)
        {
            if (options is null)
                throw new ArgumentNullException(nameof(options));

            IFormat format = new Format();
            options(format);

            if (format is null)
                throw new NullReferenceException("format cannot be null");

            return format;
        }
    }
}

