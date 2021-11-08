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
        static readonly byte[] TRACKS = { 0x16, 0x54, 0xAE, 0x6B };

        static readonly byte[] TRACKENTRY = { 0xAE };

        static readonly byte[] CODECID = { 0x86 };
        static readonly byte[] AUDIO = { 0xE1 };

        static readonly byte[] SAMPLING = { 0xB5 };
        static readonly byte[] CHANNELS = { 0x9F };


        static readonly byte[] CLUSTER = { 0x1f, 0x43, 0xb6, 0x75 };
        static readonly byte[] TIMECODE = { 0xE7 };
        static readonly byte[] SIMPLEBLOCK = { 0xA3 };
        


        public static short[] GetPcm(YtStream songStream)
        {
            MemoryStream auxStream = new MemoryStream(songStream.GetBuffer());
            List<short> pcmContent = new List<short>();
            //espero a que descargue algo y creo un nuevo memorystream con el mismo buffer para resetear la posicion
            while (songStream.Length == 0) { }
            //position absolute
            long posCluster = 0;

            while (posCluster != -1)
            {
                posCluster = FindPosition(auxStream, CLUSTER,true);

                if (posCluster != -1)
                {
                    EbmlReader ebmlReader = new EbmlReader(auxStream);
                    //ebmlReader.EnterContainer();
                    ebmlReader.ReadAt(posCluster);
                    long clusterSize = ebmlReader.ElementSize;
                    ebmlReader.EnterContainer();

                    //waits to download the first cluster
                    while (songStream.DownloadedBytes < posCluster + clusterSize) { }

                    //position relative to the cluster position
                    long posBlock = 0;
                    long startPos = FindPosition(auxStream, SIMPLEBLOCK, false);
                    OpusDecoder opusDecoder = new OpusDecoder(48000, 2);
                    bool isError = false;
                    //bool isEnd = false;
                    while (posBlock >= 0 && !isError && auxStream.Position < posCluster + clusterSize)
                    {
                        if (posBlock != 1)
                        {
                            isError = false;
                            try
                            {
                                ebmlReader.ReadAt(posBlock);
                            }
                            catch (Exception ex) { ebmlReader.LeaveContainer(); isError = true; }
                            if (!isError)
                            {
                                byte[] opusBuffer = new byte[ebmlReader.ElementSize - 4];
                                auxStream.Seek(auxStream.Position + 4, SeekOrigin.Begin);
                                ebmlReader.ReadBinary(opusBuffer, 0, opusBuffer.Length);
                                int channelCount = OpusPacketInfo.GetNumEncodedChannels(opusBuffer, 0);
                                int frames = OpusPacketInfo.GetNumFrames(opusBuffer, 0, opusBuffer.Length);
                                int frame_size = OpusPacketInfo.GetNumSamples(opusBuffer, 0, opusBuffer.Length, 48000);
                                short[] pcmBuffer = new short[frame_size * frames * channelCount];
                                try
                                {
                                    int decodedSamples = opusDecoder.Decode(opusBuffer, 0, opusBuffer.Length, pcmBuffer, 0, frame_size);
                                    pcmContent.AddRange(pcmBuffer);
                                }
                                catch (Exception ex) { }
                            }
                            posBlock = FindPosition(auxStream, SIMPLEBLOCK, false) - startPos;
                            //memoryStream.Seek(posBlock, SeekOrigin.Begin);

                        }

                    }
                    auxStream.Seek(ebmlReader.ElementPosition, SeekOrigin.Begin);
                    //isEnd = (ebmlReader.ElementPosition + ebmlReader.ElementSize) >= memoryStream.Length;
                    ebmlReader.LeaveContainer();
                }

            }

            return pcmContent.ToArray();
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
