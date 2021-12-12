using System;
using WebmOpus;
using WebmOpus.Exceptions;
using Xunit;

namespace WebmOpusUnitTest
{
    public class WebmOpusTest
    {
        [Theory]
        [InlineData(VideoIds.Normal)]
        [InlineData(VideoIds.AgeRestricted)]
        [InlineData(VideoIds.RatingDisabled)]
        [InlineData(VideoIds.EmbedRestrictedByAuthor)]
        [InlineData(VideoIds.EmbedRestrictedByYouTube)]
        [InlineData(VideoIds.ContainsClosedCaptions)]
        [InlineData(VideoIds.ContainsDashManifest)]
        [InlineData(VideoIds.ContainsHighQualityStreams)]
        [InlineData(VideoIds.LiveStreamRecording)]
        [InlineData(VideoIds.HighDynamicRange)]
        [InlineData(VideoIds.Omnidirectional)]
        public void ClustersShouldDownload (string videoId)
        {
            WebmToOpus webmToOpus = new WebmToOpus(videoId);
            var clusters = webmToOpus.GetClusters().GetAwaiter().GetResult();
            Assert.True(clusters.Count > 0 && clusters.Count == webmToOpus.ClusterPositions.Count);
        }
        [Theory]
        [InlineData(VideoIds.NonExisting)]
        [InlineData(VideoIds.Private)]
        [InlineData(VideoIds.Unlisted)]
        public void ClustersShouldThrowException (string videoId)
        {
            Assert.Throws<YtStreamException>(() => new WebmToOpus(videoId));   
        }
    }
}
