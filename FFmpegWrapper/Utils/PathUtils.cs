using System;
using System.IO;

namespace FFmpegWrapper.Helpers
{
    internal static class PathUtils
    {
        internal static string TryGetFFmpegPath() => TryGetFFPath("ffmpeg");
        internal static string TryGetFFprobePath() => TryGetFFPath("ffprobe");
        private static string TryGetFFPath(string name)
        {

            if (OperatingSystem.IsLinux())
                return name;

            var files = Directory.EnumerateFiles(Directory.GetCurrentDirectory());
            foreach (var item in files)
            {
                if (Path.GetFileName(item).StartsWith(name))
                    return item;
            }
            throw new System.Exception($"{name} not found in directory");
        }
    }
}
