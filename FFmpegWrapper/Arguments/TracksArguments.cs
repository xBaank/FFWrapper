
namespace FFmpegWrapper.Arguments
{
    public static class TracksArguments
    {
        public static string WithAudioTrack(int audioTrack) => $"-map:a {audioTrack}";
        public static string WithVideoTrack(int videoTrack) => $"-map:v {videoTrack}";
        public static string WithAllAudioTracks() => $"-map 0:a";
        public static string WithAllVideoTracks() => $"-map 0:v";
        public static string WithAllTracks() => $"-map 0";
    }
}
