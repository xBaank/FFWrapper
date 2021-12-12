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
            string normal;
            string ageRestricted;
            string containsDashManifes;
            string ratingDisabled;
            string omniDirectional;
            Assert.IsNotNull(normal = YtStream.GetSongUrl(VideoIds.Normal).GetAwaiter().GetResult());
            Assert.IsNotNull(ageRestricted = YtStream.GetSongUrl(VideoIds.AgeRestricted).GetAwaiter().GetResult());
            Assert.IsNotNull(containsDashManifes = YtStream.GetSongUrl(VideoIds.ContainsHighQualityStreams).GetAwaiter().GetResult());
            Assert.IsNotNull(ratingDisabled = YtStream.GetSongUrl(VideoIds.RatingDisabled).GetAwaiter().GetResult());
            Assert.IsNotNull(omniDirectional = YtStream.GetSongUrl(VideoIds.Omnidirectional).GetAwaiter().GetResult());
            Assert.IsTrue(IsDownloadClusterOk(new WebmToOpus(normal)));
            Assert.IsTrue(IsDownloadClusterOk(new WebmToOpus(ageRestricted)));
            Assert.IsTrue(IsDownloadClusterOk(new WebmToOpus(containsDashManifes)));
            Assert.IsTrue(IsDownloadClusterOk(new WebmToOpus(ratingDisabled)));
            Assert.IsTrue(IsDownloadClusterOk(new WebmToOpus(omniDirectional)));



        }
        private static bool IsDownloadClusterOk(WebmToOpus webmToOpus)
        {
            webmToOpus.DownloadClusterPositions().Wait();
            return webmToOpus.ClusterPositions.Count > 0;
        }
    }
}
