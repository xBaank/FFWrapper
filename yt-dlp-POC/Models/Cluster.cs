using System;
using System.Collections.Generic;
using System.Text;

namespace yt_dlp_POC.Models
{
    public class Cluster
    {
        internal Cluster(List<OpusPacket>packets ,ulong timeStamp)
        {
            TimeStamp = timeStamp;
            Packets = packets;
        }
        public List<OpusPacket> Packets { get; }
        public ulong TimeStamp { get; }
    }
}
