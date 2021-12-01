using System;
using System.Collections.Generic;
using System.Text;
using WebmOpus;

namespace WebmOpusUnitTest
{
    internal static class VideoIds
    {
        public static WebmToOpus Normal = new WebmToOpus(new YtStream(YtStream.GetSongUrl("9bZkp7q19f0").GetAwaiter().GetResult()));
        public static WebmToOpus Unlisted = new WebmToOpus(new YtStream(YtStream.GetSongUrl("IpZhObvHGJA").GetAwaiter().GetResult()));
        public static WebmToOpus AgeRestricted = new WebmToOpus(new YtStream(YtStream.GetSongUrl("rXMX4YJ7Lks").GetAwaiter().GetResult()));
        public static WebmToOpus NonExisting = new WebmToOpus(new YtStream(YtStream.GetSongUrl("qld9w0b").GetAwaiter().GetResult()));
        public static WebmToOpus HighDynamicRange = new WebmToOpus(new YtStream(YtStream.GetSongUrl("vX2vsvdq8nw").GetAwaiter().GetResult()));
        public static WebmToOpus Omnidirectional = new WebmToOpus(new YtStream(YtStream.GetSongUrl("bJQ4vI").GetAwaiter().GetResult()));
        public static WebmToOpus ContainsDashManifest = new WebmToOpus(new YtStream(YtStream.GetSongUrl("AI7ULzgf8RU").GetAwaiter().GetResult()));
        public static WebmToOpus ContainsHighQualityStreams = new WebmToOpus(new YtStream(YtStream.GetSongUrl("V5Fsj_sCKdg").GetAwaiter().GetResult()));
        // TODO
        //public static WebmToOpus LiveStream = new WebmToOpus(new YtStream(YtStream.GetSongUrl("5qap5aO4i9A").GetAwaiter().GetResult()));
        public static WebmToOpus RatingDisabled = new WebmToOpus(new YtStream(YtStream.GetSongUrl("5VGm0dczmHc").GetAwaiter().GetResult()));
        public static WebmToOpus ContainsClosedCaptions = new WebmToOpus(new YtStream(YtStream.GetSongUrl("YltHGKX80Y8").GetAwaiter().GetResult()));
        //public const string Unlisted = "IpZhObvHGJA";
        //public const string Private = "pb_hHv3fByo";
        //public const string NonExisting = "qld9w0b-1ao";
        //public const string LiveStream = "5qap5aO4i9A";
        //public const string LiveStreamRecording = "rsAAeyAr-9Y";
        //public const string ContainsHighQualityStreams = "V5Fsj_sCKdg";
        //public const string ContainsDashManifest = "AI7ULzgf8RU";
        //public const string Omnidirectional = "-xNN-bJQ4vI";
        //public const string HighDynamicRange = "vX2vsvdq8nw";
        //public const string ContainsClosedCaptions = "YltHGKX80Y8";
        //public const string EmbedRestrictedByYouTube = "_kmeFXjjGfk";
        //public const string EmbedRestrictedByAuthor = "MeJVWBSsPAY";
        //public const string AgeRestricted = "SkRSXFQerZs";
        //public const string AgeRestrictedEmbedRestricted = "hySoCSoH-g8";
        //public const string RatingDisabled = "5VGm0dczmHc";
        //public const string RequiresPurchase = "p3dDcKOFXQg";
    }
}
