using System;
using System.Collections.Generic;
using System.Text;

namespace yt_dlp_POC
{
     internal class OpusFormat
    {
        public OpusFormat(float sz,int ch)
        {
            sampleFrequency = sz;
            channels = ch;
        }
        public float sampleFrequency { get; }
        public int channels { get; }
    }
}
