using Concentus.Structs;
using System;
using System.Collections.Generic;
using System.Text;

namespace yt_dlp_POC
{
    public class OpusPacket
    {
        internal OpusPacket(byte[] buffer,int timeSpan)
        {
            OpusBuffer = buffer;
            TimeSpan = timeSpan;
        }
        public int ChannelCount { get { return OpusPacketInfo.GetNumEncodedChannels(OpusBuffer, 0); } }
        public int Frames { get { return OpusPacketInfo.GetNumFrames(OpusBuffer, 0, OpusBuffer.Length); } }
        public int FrameSize { get { return OpusPacketInfo.GetNumSamples(OpusBuffer, 0, OpusBuffer.Length, 48000); } }
        public byte[] OpusBuffer { get; }
        public int TimeSpan { get; }
    }
}
