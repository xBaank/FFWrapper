
namespace FFmpegWrapper.Builders
{
    public static class TracksArgumentsBuilder
    {
        public static string WithTrackAudioTrack(int audioTrack) => $"-map:a {audioTrack}";
        public static string WithTrackVideoTrack(int videoTrack) => $"-map:v {videoTrack}";
        public static string WithAllTracks() => $"-map 0";
    }
}
