using System;
using System.Collections.Generic;
using System.Text;

namespace WebmOpus.Models
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
