using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NEbml.Core;

namespace yt_dlp_POC
{
    public class OpusToPcm
    {
        //1f43b675
        static readonly byte[] CLUSTER = { 0x1f, 0x43,0xb6,0x75 };
        static readonly byte[] TIMECODE = { 0xE7 };
        static readonly byte[] BLOCK = { 0xA3 };
        const byte SIZE1 = 0x80; // 1 byte
        const byte SIZE2 = 0x40; // 2 bytes
        const byte SIZE3 = 0x20; // 3 bytes

        public static byte[] GetPcm(MemoryStream stream)
        {
            EbmlReader ebmlReader = new EbmlReader(stream);
            MemoryStream memoryStream = new MemoryStream(stream.GetBuffer());
            //espero a que descargue algo y creo un nuevo memorystream con el mismo buffer para resetear la posicion
            while (stream.Length == 0) { }
            long posCluser = 0;

            while (posCluser != -1)
            {
                posCluser = FindPosition(memoryStream, CLUSTER);
                //size del size xd

                uint size = GetSize(memoryStream);

                long posBlock = 0;

                while(posBlock != -1 && memoryStream.Position < posCluser + size )
                {
                    posBlock = FindPosition(memoryStream, BLOCK);

                    uint PosBlocksize = GetSize(memoryStream);


                }

            }
            
            return CLUSTER;
        }

        public static uint GetSize(MemoryStream memoryStream)
        {
            byte sizeSize = (byte)memoryStream.ReadByte();
            //resta cantidad para quitar la parte del size(que es el primer bit)
            byte firstByteRest = 0;

            if (sizeSize - SIZE3 < 16)
            {
                sizeSize = 3;
                firstByteRest = SIZE3;
            }
            else if (sizeSize - SIZE2 < 16)
            {
                sizeSize = 2;
                firstByteRest = SIZE2;

            }
            else if (sizeSize - SIZE1 < 16)
            {
                sizeSize = 1;
                firstByteRest = SIZE1;
            }


            byte[] buffer = new byte[4];
            //Desde  donde tiene que empezar escribir en el buffer.
            byte shiftRightSize = (byte)(4 - sizeSize);
            //resto uno a la posicion para que vuelva a leer la parte del "size del size" y luego le resto lo que ocupa
            memoryStream.Seek(memoryStream.Position - 1, SeekOrigin.Begin);
            memoryStream.Read(buffer, shiftRightSize, sizeSize);
            buffer[shiftRightSize] -= firstByteRest;
            //si es littleendian se hace un reverse ya que lo lee al reves.
            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(buffer);
            }
            return BitConverter.ToUInt32(buffer, 0);
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
