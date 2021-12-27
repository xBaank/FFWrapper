using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Threading;

namespace WebmOpus
{
    public class WebmOpusStream : MemoryStream
    {
        private const int CLUSTERPOSLENGHT = 1024 * 100;
        private HttpClient httpClient;

        public bool ClusterPositionsDownloaded { get; private set; }
        public int Size { get; private set; }


        public WebmOpusStream(string url) : base(GetSize(url))
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            Size = Capacity;
        }

        private static int GetSize(string url)
        {
            bool isError = true;
            long length = 0;
            for (int i = 0; i < 5 && isError; i++)
            {
                try
                {
                    var request = WebRequest.CreateHttp(url);
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
            ClusterPositionsDownloaded = !cancellationToken.IsCancellationRequested;
            if (cancellationToken.IsCancellationRequested)
                throw new OperationCanceledException();
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
