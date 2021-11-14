using System;
using System.Collections.Generic;
using System.IO;
using YoutubeExplode;
using System.Text;
using System.Linq;
using YoutubeExplode.Videos.Streams;
using System.Net.Http;
using System.Threading.Tasks;

namespace yt_dlp_POC
{
    public class YtDownloader
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="query"></param>
        /// <returns>El stream sin reserva de memoria</returns>
        public async static Task<YtStream> DownloadSong(string query)
        {
            YoutubeClient youtubeClient = new YoutubeClient();
            HttpClient httpClient = new HttpClient();
            var queryresult = await youtubeClient.Search.GetVideosAsync(query).FirstOrDefaultAsync();


            var video = await youtubeClient.Videos.Streams.GetManifestAsync(queryresult.Id);
            var streamInfo = video.GetAudioOnlyStreams().Where(i => i.Container == Container.WebM && i.AudioCodec == "opus").FirstOrDefault();
            return new YtStream(streamInfo.Url) ;
        }
    }
}
