using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using YoutubeExplode;
using YoutubeExplode.Playlists;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace WebmPOC
{
    public class YtUtils
    {
        /// <summary>
        /// Gives first url song from youtube
        /// </summary>
        /// <param name="query"></param>
        /// <returns>streaminfo or null if not found</returns>
        public static async Task<List<Video>> GetSongsUrl(string query)
        {
            List<Video> videos = new List<Video>();    
            YoutubeClient youtubeClient = new YoutubeClient();
            var playlistid = PlaylistId.TryParse(query);
            if (string.IsNullOrEmpty(playlistid))
            {
                var queryresult = await youtubeClient.Search.GetVideosAsync(query).FirstOrDefaultAsync() ?? throw new Exception("Url or query not found");

                videos.Add(new Video(queryresult.Id,queryresult.Title,queryresult.Author,default,default,queryresult.Duration,queryresult.Thumbnails,default,default));
            }
            else
            {
                videos = await youtubeClient.Playlists.GetVideosAsync(PlaylistId.Parse(playlistid)).Select(i => new Video(i.Id,i.Title,i.Author,default,default,i.Duration,i.Thumbnails,default,default) ).ToListAsync();
                
            }
            return videos;
        }
        public static async Task<IStreamInfo> GetStreamInfo(string videoId)
        {
            YoutubeClient youtubeClient = new YoutubeClient();
            AudioOnlyStreamInfo? audio = null;
            bool retry = true;
            int retries = 5;
            while (retries > 0 && retry == true)
            {
                try
                {
                    var streamManifest = await youtubeClient.Videos.Streams.GetManifestAsync(videoId);
                    audio = streamManifest.GetAudioOnlyStreams().Where(i => i.Container == Container.WebM && i.AudioCodec == "opus").OrderBy(i => i.Bitrate.BitsPerSecond).FirstOrDefault();
                    retry = false;
                }
                catch 
                {
                    retry = true;
                    retries--;
                    Thread.Sleep(1000);
                }
            }
            if (audio is null)
                throw new Exception("Playable streams not found");
            return audio;
        }
    }
}
