using Microsoft.VisualStudio.TestTools.UnitTesting;
using WebmOpus;

namespace WebmOpusUnitTest
{
    [TestClass]
    public class WebmOpusTest
    {
        [TestMethod]
        public void DownloadClustersPositionsTest()
        {
            Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.Normal));
            //Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.LiveStream));
            Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.AgeRestricted));
            Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.ContainsDashManifest));
            Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.ContainsHighQualityStreams));
            Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.RatingDisabled));
            Assert.AreEqual(true,IsDownloadClusterOk(VideoIds.Omnidirectional));
        }
        private static bool IsDownloadClusterOk(WebmToOpus webmToOpus)
        {
            webmToOpus.DownloadClusterPositions().Wait();
            return webmToOpus.ClusterPositions.Count > 0;
        }
    }
}
