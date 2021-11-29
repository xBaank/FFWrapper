using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebmOpus;

namespace WebmOpusUnitTest
{
    [TestClass]
    public class WebmOpusTest
    {
        [TestMethod]
        public void DownloadVideos()
        {
            WebmToOpus webmToOpus = new WebmToOpus(new YtStream(YtStream.GetSongUrl("3434").Result));
            webmToOpus.DownloadClusterPositions().Wait();
            var clustersPositions = webmToOpus.ClusterPositions;
            var clusters = webmToOpus.GetClusters().Result;
            Assert.IsNotNull(clustersPositions);
        }
    }
}
