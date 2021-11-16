using System;
using System.Collections.Generic;
using System.Text;

namespace WebmOpus.Models
{
    internal class CluesterPosition
    {
        public CluesterPosition(ulong timestamp, ulong clusterposition)
        {
            TimeStamp = timestamp;
            ClusterPosition = clusterposition;
        }
        public ulong TimeStamp { get; }
        public ulong ClusterPosition { get; }
    }
}
