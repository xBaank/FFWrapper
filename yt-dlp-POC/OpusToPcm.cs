using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Concentus.Structs;
using NEbml.Core;

namespace yt_dlp_POC
{
    public class OpusToPcm
    {
        //1f43b675
        static readonly byte[] CLUSTER = { 0x1f, 0x43, 0xb6, 0x75 };
        static readonly byte[] TIMECODE = { 0xE7 };
        static readonly byte[] BLOCK = { 0xA3 };
        const byte SIZE1 = 0x80; // 1 byte
        const byte SIZE2 = 0x40; // 2 bytes
        const byte SIZE3 = 0x20; // 3 bytes

        public static short[] GetPcm(MemoryStream stream)
        {
            MemoryStream memoryStream = new MemoryStream(stream.GetBuffer());
            List<short> pcmContent = new List<short>();
            //espero a que descargue algo y creo un nuevo memorystream con el mismo buffer para resetear la posicion
            while (stream.Length == 0) { }
            long posCluser = 0;

            while (posCluser != -1)
            {
                posCluser = FindPosition(memoryStream, CLUSTER,true);

                EbmlReader ebmlReader = new EbmlReader(memoryStream);
                //ebmlReader.EnterContainer();
                var cluster = ebmlReader.ReadAt(posCluser);
                long clusterSize = ebmlReader.ElementSize;
                ebmlReader.EnterContainer();

                long posBlock = 0;
                long startPos = FindPosition(memoryStream, BLOCK, false);
                OpusDecoder opusDecoder = new OpusDecoder(48000, 2);
                bool isError = false;
                bool isEnd = false;
                while (posBlock < clusterSize && !isError && !isEnd)
                {
                    if (posBlock != 1)
                    {
                        isError = false;
                        try
                        {
                            ebmlReader.ReadAt(posBlock);
                        }
                        catch(Exception ex) { ebmlReader.LeaveContainer(); isError = true; }
                        if (!isError)
                        {
                            Thread.Sleep(2);
                            byte[] opusBuffer = new byte[ebmlReader.ElementSize - 4];
                            memoryStream.Seek(memoryStream.Position + 4, SeekOrigin.Begin);
                            ebmlReader.ReadBinary(opusBuffer, 0, opusBuffer.Length);
                            int channelCount = OpusPacketInfo.GetNumEncodedChannels(opusBuffer, 0);
                            var a = OpusPacketInfo.GetEncoderMode(opusBuffer, 0);
                            int frames = OpusPacketInfo.GetNumFrames(opusBuffer, 0, opusBuffer.Length);
                            int frame_size = OpusPacketInfo.GetNumSamples(opusBuffer, 0, opusBuffer.Length, 48000);
                            short[] pcmBuffer = new short[frame_size * frames * channelCount];
                            try
                            {
                                int decodedSamples = opusDecoder.Decode(opusBuffer, 0, opusBuffer.Length, pcmBuffer, 0, frame_size);
                                pcmContent.AddRange(pcmBuffer);
                            }
                            catch(Exception ex) { }
                        }
                        posBlock = FindPosition(memoryStream, BLOCK, false) - startPos;
                        //memoryStream.Seek(posBlock, SeekOrigin.Begin);

                    }

                }
                memoryStream.Seek(ebmlReader.ElementPosition, SeekOrigin.Begin);
                //isEnd = (ebmlReader.ElementPosition + ebmlReader.ElementSize) >= memoryStream.Length;
                ebmlReader.LeaveContainer();

            }

            return pcmContent.ToArray();
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

        public static long FindPosition(Stream stream, byte[] byteSequence, bool reset = false)
        {
            long result = -1;
            if (byteSequence.Length > stream.Length)
                return result;

            byte[] buffer = new byte[byteSequence.Length];

            int i;
            bool isFound = false;
            while ((i = stream.Read(buffer, 0, byteSequence.Length)) == byteSequence.Length && !isFound)
            {
                if (byteSequence.SequenceEqual(buffer))
                {
                    result = stream.Position - byteSequence.Length;
                    isFound = true;
                }
                else
                    stream.Position -= byteSequence.Length - PadLeftSequence(buffer, byteSequence);
            }

            if (reset && result != -1)
                stream.Seek(0, SeekOrigin.Begin);
            else if (result != -1)
                stream.Seek(result, SeekOrigin.Begin);


            return result;
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
