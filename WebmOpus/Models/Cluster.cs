using System.Collections.Generic;


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
