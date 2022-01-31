using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;

namespace WebmOpus.Models
{
    internal class FFmpegProcess : Process
    {
        internal Stream Input { get; set; }
        internal Stream Output { get; set; } = new MemoryStream();
        internal bool isOutputEventRaised { get; set; } = false;
        internal bool isErrorEventRaised { get; set; } = false;
    }
}
