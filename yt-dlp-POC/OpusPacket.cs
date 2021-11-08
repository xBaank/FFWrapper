using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace yt_dlp_POC
{
    public class OpusPacket
    {
        int channelCount = 0;
        int frames = 0;
        int frame_size = 0;
        byte[] opusBuffer;
    }
}
