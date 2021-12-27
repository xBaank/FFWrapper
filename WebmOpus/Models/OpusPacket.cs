using Concentus.Structs;

namespace WebmOpus
{
    public class OpusPacket
    {
        internal OpusPacket(byte[] buffer,int timeSpan,OpusFormat opusFormat)
        {
            OpusBuffer = buffer;
            TimeSpan = timeSpan;
            OpusFormat = opusFormat;
        }
        public OpusFormat OpusFormat { get; }
        public int ChannelCount { get { return OpusPacketInfo.GetNumEncodedChannels(OpusBuffer, 0); } }
        public int Frames { get { return OpusPacketInfo.GetNumFrames(OpusBuffer, 0, OpusBuffer.Length); } }
        public int FrameSize { get { return OpusPacketInfo.GetNumSamples(OpusBuffer, 0, OpusBuffer.Length, (int)OpusFormat.sampleFrequency); } }
        public byte[] OpusBuffer { get; }
        public int TimeSpan { get; }
    }
}
