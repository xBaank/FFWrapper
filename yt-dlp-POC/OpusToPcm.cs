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
        const int SIZE1 = 0x80; // 1 byte
        const int SIZE2 = 0x40; // 2 bytes
        const int SIZE3 = 0x20; // 3 bytes

        public static byte[] GetPcm(MemoryStream stream)
        {
            MemoryStream memoryStream = new MemoryStream(stream.GetBuffer());
            //espero a que descargue algo y creo un nuevo memorystream con el mismo buffer para resetear la posicion
            while (stream.Length == 0) { }
            long pos = FindPosition(memoryStream, CLUSTER);
            byte sizeSize = (byte)memoryStream.ReadByte();

            if (sizeSize - SIZE3 < 16)
                sizeSize = 3;
            else if (sizeSize - SIZE2 < 16)
                sizeSize = 2;
            else if (sizeSize - SIZE1 < 16)
                sizeSize = 1;

            //primero byte, primer numero es la longitud del size.
            byte[] buffer = new byte[sizeSize];
            buffer[0] = sizeSize 
            memoryStream.Read(buffer, 0, buffer.Length - 1);
            
            return CLUSTER;
        }
        public static long FindPosition(Stream stream, byte[] byteSequence)
        {
            if (byteSequence.Length > stream.Length)
                return -1;

            byte[] buffer = new byte[byteSequence.Length];

                int i;
                while ((i = stream.Read(buffer, 0, byteSequence.Length)) == byteSequence.Length)
                {
                    if (byteSequence.SequenceEqual(buffer))
                        return stream.Position - byteSequence.Length;
                    else
                        stream.Position -= byteSequence.Length - PadLeftSequence(buffer, byteSequence);
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
