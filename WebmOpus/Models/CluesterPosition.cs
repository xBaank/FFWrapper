
namespace WebmOpus.Models
{
    public class ClusterPosition
    {
        public ClusterPosition(ulong timestamp, ulong clusterposition)
        {
            TimeStamp = timestamp;
            ClusterPos = clusterposition;
        }
        public ulong TimeStamp { get; }
        public ulong ClusterPos { get; }
        public bool IsClusterDownloaded { get; internal set; }
    }
}
