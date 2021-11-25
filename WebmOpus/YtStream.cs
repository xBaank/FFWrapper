using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WebmOpus.Models;

namespace WebmOpus
{
    public class YtStream : MemoryStream
    {
        private const int CLUSTERPOSLENGHT = 1024 * 100;
        private long downloadedBytes;
        private HttpClient httpClient;
        private int i;

        public long DownloadedBytes { get { return downloadedBytes; } }
        public bool HasFinished { get; private set; }
        public bool IsComplete { get; private set; } = true;
        public bool ClusterPositionsDownloaded { get; private set; }

        public YtStream(string url) : base((int)GetSize(url).Result)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            //Empieza la descarga en otro thread.
            //Task.Run(async () => await StartDownload());
        }

        private static async Task<long> GetSize(string url)
        {
            var request = HttpWebRequest.CreateHttp(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            var length = response.ContentLength;
            return length;
        }

        internal async Task<byte[]> DownloadClusterPositions()
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(0, CLUSTERPOSLENGHT - 1);


            var responseMessage = await httpClient.SendAsync(httpRequestMessage);
            byte[] auxBuffer = await responseMessage.Content.ReadAsByteArrayAsync();
            base.Position = 0;
            base.Write(auxBuffer, 0, auxBuffer.Length);
            ClusterPositionsDownloaded = true;

            return auxBuffer;
        }

        internal async Task<byte[]> DownloadCluster(ulong from, ulong to)
        {
            HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
            httpRequestMessage.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue((long?)from, (long?)to);


            var responseMessage = await httpClient.SendAsync(httpRequestMessage);
            byte[] auxBuffer = await responseMessage.Content.ReadAsByteArrayAsync();
            base.Position = (long)from;
            base.Write(auxBuffer, 0, auxBuffer.Length);
            ClusterPositionsDownloaded = true;

            return auxBuffer;
        }

    }
}
