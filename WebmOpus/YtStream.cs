using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebmOpus.Models;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
using YoutubeExplode.Exceptions;
using System.Threading;
using WebmOpus.Exceptions;

namespace WebmOpus
{
    public class YtStream : MemoryStream
    {
        private const int CLUSTERPOSLENGHT = 1024 * 100;
        private HttpClient httpClient;

        public bool ClusterPositionsDownloaded { get; private set; }
        public int Size { get; private set; }
        public int? Bitrate { get; private set; }

        /// <summary>
        /// Da la url de descarga
        /// </summary>
        /// <param name="query"></param>
        /// <returns>Url de descarga o nulo si no se pudo encontrar</returns>
        public async static Task<IStreamInfo> GetSongUrl(string query)
        {
                YoutubeClient youtubeClient = new YoutubeClient();
                var queryresult = await youtubeClient.Search.GetVideosAsync(query).FirstOrDefaultAsync();

                if (queryresult == null)
                    throw new YtStreamException($"Couldn't find any video for {query}");
                var video = await youtubeClient.Videos.Streams.GetManifestAsync(queryresult.Id);
                var streamInfo = video.GetAudioOnlyStreams().Where(i => i.Container == Container.WebM && i.AudioCodec == "opus").OrderBy(i => i.Bitrate.BitsPerSecond).FirstOrDefault();
                return streamInfo;
        }

        public YtStream(string urlOrQuery,int? bitrate = null) : base(GetSize(ref urlOrQuery,ref bitrate))
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(urlOrQuery);
            Size = Capacity;
            Bitrate = bitrate;
        }

        private static int GetSize(ref string urlOrQuery,ref int? bitrate)
        {
            var info = GetSongUrl(urlOrQuery).GetAwaiter().GetResult();
            urlOrQuery = info.Url;
            bitrate = (int?)info.Bitrate.BitsPerSecond;
            bool isError = true;
            long length = 0;
            for (int i = 0; i < 5 && isError; i++)
            {
                try
                {
                    var request = WebRequest.CreateHttp(urlOrQuery);
                    request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
                    var response = (HttpWebResponse)(request.GetResponseAsync().GetAwaiter().GetResult());
                    length = response.ContentLength;
                    isError = false;
                }
                catch
                {
                    isError = true;
                    Thread.Sleep(1000);
                }
            }

            if (isError)
                throw new YtStreamException($"Couldn't get size for {urlOrQuery}");

            return (int)length;
        }

        internal async Task<byte[]> DownloadClusterPositions(CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, CLUSTERPOSLENGHT - 1);


            var responseMessage = await httpClient.SendAsync(httpRequestMessage,cancellationToken);
            byte[] auxBuffer = await responseMessage.Content.ReadAsByteArrayAsync();
            base.Position = 0;
            base.Write(auxBuffer, 0, auxBuffer.Length);
            ClusterPositionsDownloaded = true;

            return auxBuffer;
        }

        internal async Task<byte[]> DownloadCluster(ulong from, ulong to,CancellationToken cancellationToken = default)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue((long?)from, (long?)to);


            var responseMessage = await httpClient.SendAsync(httpRequestMessage,cancellationToken);
            byte[] auxBuffer = await responseMessage.Content.ReadAsByteArrayAsync();
            base.Position = (long)from;
            base.Write(auxBuffer, 0, auxBuffer.Length);
            ClusterPositionsDownloaded = true;

            return auxBuffer;
        }

    }
}
