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

        static readonly byte[] TRACKS = { 0x16, 0x54, 0xAE, 0x6B };

        static readonly byte[] TRACKENTRY = { 0xAE };

        static readonly byte[] CODECID = { 0x86 }; // A_OPUS String
        static readonly byte[] AUDIO = { 0xE1 };

        static readonly byte[] SAMPLING = { 0xB5 }; //float
        static readonly byte[] CHANNELS = { 0x9F }; //uint


        static readonly byte[] CLUSTER = { 0x1f, 0x43, 0xb6, 0x75 };
        static readonly byte[] TIMECODE = { 0xE7 }; //binary
        static readonly byte[] SIMPLEBLOCK = { 0xA3 }; //binary

        const int ERRORCODE = -1;
        const string CODECNAME = "A_OPUS";

        /// <summary>
        /// Extrae el formato de audio de opus
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns>Formato de audio</returns>
        private static OpusFormat GetOpusFormat(MemoryStream memoryStream)
        {
            OpusFormat opusFormat = null;
            //------------BUSCA LA POSICION Y RESETEA EL STREAM-----------

            bool isFound = false;
            string codec = String.Empty;
            long posAudio = 0;
            while (!isFound && posAudio != ERRORCODE)
            {
                try
                {
                    EbmlReader ebmlReader = new EbmlReader(memoryStream);
                    posAudio = FindPosition(memoryStream, AUDIO, true);
                    ebmlReader.ReadAt(posAudio);
                    ebmlReader.EnterContainer();
                    //------------BUSCA LA PRIMERA DEL PRIMER ELEMENTO DENTRO DEL CONTENEDOR-----------
                    long startPos = FindPosition(memoryStream, SAMPLING);
                    //---LEE EL PRIMER ELEMENTO---
                    ebmlReader.ReadAt(0);
                    float sampling = (float)ebmlReader.ReadFloat();
                    long posChannels = FindPosition(memoryStream, CHANNELS) - startPos;
                    //---LEE EL SEGUNDO ELEMENTO---
                    ebmlReader.ReadAt(posChannels);
                    int channels = (int)ebmlReader.ReadInt();
                    opusFormat = new OpusFormat(sampling, channels);
                    ebmlReader.LeaveContainer();
                    isFound = true;
                }
                catch
                {
                    isFound = false;
                    memoryStream.Seek(posAudio + 1, SeekOrigin.Begin);
                }
            }
            return opusFormat;

        }
        /// <summary>
        /// Checkea si es opus
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns>true si es soportado</returns>
        private static bool IsSupportedCodec(MemoryStream memoryStream)
        {
            bool isFound = false;
            string codec = String.Empty;
            long posTracks = 0;
            while (!isFound && posTracks != ERRORCODE)
            {
                try
                {
                    EbmlReader ebmlReader = new EbmlReader(memoryStream);

                    posTracks = FindPosition(memoryStream, TRACKS, true);
                    ebmlReader.ReadAt(posTracks);
                    ebmlReader.EnterContainer();
                    long posTrackEntry = FindPosition(memoryStream, TRACKENTRY);
                    ebmlReader.ReadAt(0);
                    ebmlReader.EnterContainer();
                    long posCodec = FindPosition(memoryStream, CODECID);
                    ebmlReader.ReadAt(0);
                    codec = ebmlReader.ReadUtf();
                    isFound = true;
                }
                catch
                {
                    isFound = false;
                    memoryStream.Seek(posTracks + 1, SeekOrigin.Begin);
                };
            }
            return codec == CODECNAME;
        }

        public static Queue<OpusPacket> GetPackets(YtStream songStream)
        {
            MemoryStream auxStream = new MemoryStream(songStream.GetBuffer());
            Queue<OpusPacket> opusContent = new Queue<OpusPacket>();

            //--------------ESPERA A QUE SE DESCARGUE MEDIO MB----------------
            WaitForDownloadedBytes(songStream, 1024 / 2);
            if (IsSupportedCodec(auxStream))
            {
                OpusFormat opusFormat = GetOpusFormat(auxStream);
                long posCluster = 0;

                while (posCluster != ERRORCODE)
                {
                    posCluster = FindPosition(auxStream, CLUSTER, true);

                    if (posCluster != ERRORCODE)
                    {
                        EbmlReader ebmlReader = new EbmlReader(auxStream);
                        long clusterSize = EnterCluster(ebmlReader, posCluster);

                        long nextClusterPos = clusterSize + posCluster;

                        WaitForDownloadedBytes(songStream, posCluster + clusterSize);

                        //-------------POSBLOCK ES RELATIVO A LA POSICION DEL PRIMER BLOQUE----------------

                        long posBlock = 0;
                        long startPos = FindPosition(auxStream, SIMPLEBLOCK);
                        OpusDecoder opusDecoder = new OpusDecoder((int)opusFormat.sampleFrequency, opusFormat.channels);
                        bool isError = false;
                        while (!isError)
                        {
                            //------------SI LA POSICION DEL STREAM ES MAYOR A A LA DEL SIGUIENTE CLUSTER ENTONCES ERA EL ULTIMO CLUSTER--------
                            isError = auxStream.Position > nextClusterPos;

                            if (!isError)
                            {
                                try
                                {
                                    ebmlReader.ReadAt(posBlock);
                                }
                                catch (Exception ex) { ebmlReader.LeaveContainer(); isError = true; }

                                if (!isError)
                                {
                                    OpusPacket opusPacket = new OpusPacket(GetBuffer(ebmlReader, auxStream));
                                    opusContent.Enqueue(opusPacket);
                                }
                                //---------------COMO LA POSICION DEL BLOQUE ES RELATIVA AL PRIMER BLOQUE SE RESTA LA POSICION--------
                                posBlock = FindPosition(auxStream, SIMPLEBLOCK) - startPos;
                                //---------------SI POSBLOCK ES NEGATIVO ENTONCES NO HA ENCONTRADO SIGUENTE BLOQUE------------
                                isError = posBlock < 0;

                            }

                        }
                        //------------------RESETEA LA POSICION AL BLOQUE Y SALE DEL CONTENEDOR--------------------
                        auxStream.Seek(ebmlReader.ElementPosition, SeekOrigin.Begin);
                        ebmlReader.LeaveContainer();
                    }

                }
            }
            return opusContent;
        }
        /// <summary>
        /// Decodifica los paquetes opus
        /// </summary>
        /// <param name="opusPackets"></param>
        /// <returns>Array de bytes decodificado</returns>
        public static byte[] GetPcm(Queue<OpusPacket> opusPackets)
        {
            OpusDecoder opusDecoder = new OpusDecoder(48000, 2);
            List<byte> pcm = new List<byte>();

            while (opusPackets.Count > 0)
            {
                try
                {
                    OpusPacket opusPacket = opusPackets.Dequeue();
                    short[] pcmBuffer = new short[opusPacket.ChannelCount * opusPacket.Frames * opusPacket.FrameSize];
                    int decodedSamples = opusDecoder.Decode(opusPacket.OpusBuffer, 0, opusPacket.OpusBuffer.Length, pcmBuffer, 0, opusPacket.FrameSize);
                    byte[] pcmBufferInBytes = new byte[pcmBuffer.Length * 2];
                    Buffer.BlockCopy(pcmBuffer, 0, pcmBufferInBytes, 0, pcmBufferInBytes.Length);
                    pcm.AddRange(pcmBufferInBytes);
                }

                catch (Exception ex) { }
            }
            return pcm.ToArray();
        }

        /// <summary>
        /// Extrae los datos del bloque
        /// </summary>
        /// <param name="ebmlReader"></param>
        /// <param name="auxStream"></param>
        /// <returns>array de bytes con los datos del bloque</returns>
        private static byte[] GetBuffer(EbmlReader ebmlReader, Stream auxStream)
        {
            byte[] opusBuffer = new byte[ebmlReader.ElementSize - 4];
            auxStream.Seek(auxStream.Position + 4, SeekOrigin.Begin);
            ebmlReader.ReadBinary(opusBuffer, 0, opusBuffer.Length);
            return opusBuffer;
        }
        /// <summary>
        /// Espera hasta que se descargue cierta cantidad de bytes
        /// </summary>
        /// <param name="songStream">stream que esta descargando los datos</param>
        /// <param name="bytes">bytes hasta los que esperar</param>
        private static void WaitForDownloadedBytes(YtStream songStream, long bytes)
        {
            while (songStream.DownloadedBytes < bytes) { }

        }
        /// <summary>
        /// Entra al cluster
        /// </summary>
        /// <param name="ebmlReader"></param>
        /// <param name="posCluster">Posicion del cluster</param>
        /// <returns></returns>
        private static long EnterCluster(EbmlReader ebmlReader, long posCluster)
        {
            ebmlReader.ReadAt(posCluster);
            long clusterSize = ebmlReader.ElementSize;
            ebmlReader.EnterContainer();
            return clusterSize;

            //waits to download the first cluster
        }

        private static long FindPosition(Stream stream, byte[] byteSequence, bool reset = false)
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
