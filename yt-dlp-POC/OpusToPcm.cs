using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace yt_dlp_POC
{
    public class OpusToPcm
    {
        //1f43b675
        static readonly byte[] CLUSTER = { 0x1f, 0x43,0xb6,0x75 };
        readonly byte[] TIMECODE = { 0xE7 };
        readonly byte[] BLOCK = { 0xA3 };

        
        public static byte[] GetPcm(Stream stream)
        {
            long pos = FindPosition(stream, CLUSTER);
            byte[] buffer = new byte[4];
            stream.Read(buffer, 0, buffer.Length);
            long size = BitConverter.ToUInt32(buffer,0);
            
            return CLUSTER;
        }
        public static long FindPosition(Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
                return -1;

            byte[] buffer = new byte[byteSequence.Length];

            using (BufferedStream bufStream = new BufferedStream(stream, byteSequence.Length))
            {
                int i;
                while ((i = bufStream.Read(buffer, 0, byteSequence.Length)) == byteSequence.Length)
                {
                    if (byteSequence.SequenceEqual(buffer))
                        return bufStream.Position - byteSequence.Length;
                    else
                        bufStream.Position -= byteSequence.Length - PadLeftSequence(buffer, byteSequence);
                }
            }

            return -1;
        }

        private static int PadLeftSequence(byte[] bytes, byte[] seqBytes)
        {
            int i = 1;
            while (i < bytes.Length)
            {
                int n = bytes.Length - i;
                byte[] aux1 = new byte[n];
                byte[] aux2 = new byte[n];
                Array.Copy(bytes, i, aux1, 0, n);
                Array.Copy(seqBytes, aux2, n);
                if (aux1.SequenceEqual(aux2))
                    return i;
                i++;
            }
            return i;
        }
    }
    
}
