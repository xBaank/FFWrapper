using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace yt_dlp_POC
{
    public class OpusPacket
    {
        public OpusPacket(byte[] buffer)
        {
            OpusBuffer = buffer;
        }
        public int ChannelCount { get { return OpusPacketInfo.GetNumEncodedChannels(OpusBuffer, 0); } }
        public int Frames { get { return OpusPacketInfo.GetNumFrames(OpusBuffer, 0, OpusBuffer.Length); } }
        public int FrameSize { get { return OpusPacketInfo.GetNumSamples(OpusBuffer, 0, OpusBuffer.Length, 48000); } }
        public byte[] OpusBuffer { get; }
    }
}
