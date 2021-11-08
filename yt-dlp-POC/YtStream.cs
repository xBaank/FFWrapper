using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace yt_dlp_POC
{
    public class YtStream : MemoryStream
    {
        private const int BUFFERLENGTH = 1024;
        private Stream stream;
        public long DownloadedBytes { get { return downloadedBytes; } }
        private long downloadedBytes;
        public YtStream(Stream stream,int size) : base(size)
        {
            this.stream = stream;
            //Empieza la descarga en otro thread.
            Task.Run(() => StartDownload());
        }

        private Task StartDownload()
        {
            byte[] auxBuffer = new byte[BUFFERLENGTH];
            int chunkNumber = (int)Math.Ceiling((double) Capacity / BUFFERLENGTH);

            for (int i = 0; i < chunkNumber; i++)
            {
                stream.Read(auxBuffer, 0, auxBuffer.Length);
                base.Write(auxBuffer, 0, auxBuffer.Length);
                downloadedBytes = Position;
            }
            return Task.CompletedTask;
        }

    }
}
