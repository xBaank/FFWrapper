﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace yt_dlp_POC
{
    public class YtStream : MemoryStream
    {
        private const int BUFFERLENGTH = 1024;
        private long downloadedBytes;
        private HttpClient httpClient;

        public long DownloadedBytes { get { return downloadedBytes; } }
        public bool HasFinished { get; private set; }   

        public YtStream(string url) : base((int)GetSize(url).Result)
        {
            httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(url);
            //Empieza la descarga en otro thread.
            Task.Run(async () => await StartDownload());
        }

        public static async Task<long> GetSize(string url)
        {
            var request = HttpWebRequest.CreateHttp(url);
            request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:40.0) Gecko/20100101 Firefox/40.1";
            var response = (HttpWebResponse)(await request.GetResponseAsync());
            var length = response.ContentLength;
            return length;
        }

        private async Task StartDownload()
        {
            int chunkNumber = (int)Math.Ceiling((double) Capacity / BUFFERLENGTH);
            for (int i = 0; i < chunkNumber; i++)
            {
                //ultimo chunk tiene length diferente
                HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
                httpRequestMessage.Headers.Range = new System.Net.Http.Headers.RangeHeaderValue(i * BUFFERLENGTH, i * BUFFERLENGTH + BUFFERLENGTH - 1);

                //if (i == chunkNumber - 1)
                //{
                //    auxBuffer = new byte[Capacity % BUFFERLENGTH];
                //}


                var responseMessage = await httpClient.SendAsync(httpRequestMessage);
                byte[] auxBuffer = await responseMessage.Content.ReadAsByteArrayAsync();
                //stream.Read(auxBuffer, 0, auxBuffer.Length);
                base.Write(auxBuffer, 0, auxBuffer.Length);
                downloadedBytes = i * BUFFERLENGTH + BUFFERLENGTH;
               
            }
            HasFinished = true;
        }

    }
}
