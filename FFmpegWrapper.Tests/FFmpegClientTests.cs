//using System;

//using WebmOpus;

//using WebmPOC;

//using Xunit;

//namespace WebmOpusUnitTest
//{
//    public class WebmOpusTest
//    {
//        [Theory]
//        [InlineData(VideoIds.Normal)]
//        [InlineData(VideoIds.AgeRestricted)]
//        [InlineData(VideoIds.RatingDisabled)]
//        [InlineData(VideoIds.EmbedRestrictedByAuthor)]
//        [InlineData(VideoIds.EmbedRestrictedByYouTube)]
//        [InlineData(VideoIds.ContainsClosedCaptions)]
//        [InlineData(VideoIds.ContainsDashManifest)]
//        [InlineData(VideoIds.ContainsHighQualityStreams)]
//        [InlineData(VideoIds.LiveStreamRecording)]
//        [InlineData(VideoIds.HighDynamicRange)]
//        [InlineData(VideoIds.JujutsuKaisen)]
//        [InlineData(VideoIds.Omnidirectional)]
//        [InlineData(VideoIds.PlaylistId)]
//        public void ClustersShouldDownload(string videoId)
//        {
//            var songs = YtUtils.GetSongsUrl(videoId).GetAwaiter().GetResult();
//            foreach (var song in songs)
//            {
//                var streamManifest = YtUtils.GetStreamInfo(song.Id).GetAwaiter().GetResult();
//                WebmToOpus webmToOpus = new WebmToOpus(streamManifest.Url);
//                var clusters = webmToOpus.GetClusters().GetAwaiter().GetResult();
//                Assert.True(clusters.Count > 0 && webmToOpus.ClusterPositions.Count > 0 && clusters.Count == webmToOpus.ClusterPositions.Count);
//            }
//        }
//        [Theory]
//        [InlineData(VideoIds.NonExisting)]
//        [InlineData(VideoIds.Private)]
//        [InlineData(VideoIds.Unlisted)]
//        public void YtUtilsThrowException(string videoId)
//        {
//            Assert.Throws<Exception>(() => YtUtils.GetSongsUrl(videoId).GetAwaiter().GetResult());
//        }
//    }
//}
