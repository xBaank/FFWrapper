using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Concentus.Structs;
using NEbml.Core;
using WebmOpus.Models;

namespace WebmOpus
{
    public class WebmToOpus
    {
        //--------AUDIOINFO-----------
        static readonly byte[] TRACKS = { 0x16, 0x54, 0xAE, 0x6B };

        static readonly byte[] TRACKENTRY = { 0xAE };

        static readonly byte[] CODECID = { 0x86 }; // A_OPUS String
        static readonly byte[] AUDIO = { 0xE1 };

        static readonly byte[] SAMPLING = { 0xB5 }; //float
        static readonly byte[] CHANNELS = { 0x9F }; //uint

        //--------AUDIODATA-----------
        static readonly byte[] CLUSTER = { 0x1f, 0x43, 0xb6, 0x75 };
        static readonly byte[] TIMECODE = { 0xE7 }; //binary
        static readonly byte[] SIMPLEBLOCK = { 0xA3 }; //binary

        //--------SEEKINGINFO---------
        static readonly byte[] SEEKHEAD = { 0x11, 0x4D, 0x9B, 0x74 };
        static readonly byte[] CUES = { 0x1C, 0x53, 0xBB, 0x6B }; //master
        static readonly byte[] CUEPOINT = { 0xBB }; //master
        static readonly byte[] CUETIME = { 0xB3 }; //float
        static readonly byte[] CUETRACKPOSITION = { 0xB7 }; //master
        static readonly byte[] CUETRACK = { 0xF7 }; //uint
        static readonly byte[] CUECLUSTERPOSITION = { 0xF1 }; //uint

        //--------CONSTANTES-----------
        const int ERRORCODE = -1;
        const string CODECNAME = "A_OPUS";

        //--------VARIABLES----------- 
        List<ClusterPosition> clusterPositions = new List<ClusterPosition>();
        int currentDecodingIndex = 0;
        bool clusterStarted = false;
        private YtStream ytStream;
        private bool isClustersDownloaded = false;

        //--------PROPIEDADES--------
        public bool HasFinished { get; private set; }
        public OpusFormat OpusFormat { get; private set; }
        public List<ClusterPosition> ClusterPositions { get { return clusterPositions; } }

        //--------EVENTOS-----------
        public event Func<object,Cluster,Task> OnClusterDownloaded;
        public event Func<object,OpusPacket,Task> OnPacketDownloaded;
        public event Func<object,Task> OnFinished;

        /// <summary>
        /// Empieza a extraer los paquetes del stream
        /// </summary>
        /// <param name="stream">Stream</param>
        public WebmToOpus(YtStream stream)
        {
            
            ytStream = stream;
        }

        private ulong GetSeekHead(MemoryStream memoryStream)
        {
            EbmlReader ebmlReader = new EbmlReader(memoryStream);
            long posToAdd = FindPosition(memoryStream, SEEKHEAD, true);
            ebmlReader.ReadAt(posToAdd);
            memoryStream.Seek(posToAdd + ebmlReader.ElementSize, SeekOrigin.Begin);
            return posToAdd != -1 ? (ulong)posToAdd : 0;
        }

        private void GetClusterPositions(MemoryStream memoryStream, ulong posToAdd)
        {
            EbmlReader ebmlReader = new EbmlReader(memoryStream);

            long cue = FindPosition(memoryStream, CUES, true);
            ebmlReader.ReadAt(cue);
            long size = ebmlReader.ElementSize;
            ebmlReader.EnterContainer();

            long startPos = FindPosition(memoryStream, CUEPOINT);
            long cuePoint = 0;
            ebmlReader.ReadAt(cuePoint);

            while (cuePoint != ERRORCODE && cuePoint < size)
            {

                long cuePointSize = ebmlReader.ElementSize;
                ebmlReader.EnterContainer();

                long timePoint = FindPosition(memoryStream, CUETIME) - cuePoint;

                ebmlReader.ReadAt(0);
                ulong timeStamp = ebmlReader.ReadUInt();

                long cueTrackPoint = FindPosition(memoryStream, CUETRACKPOSITION);

                ebmlReader.ReadAt(cueTrackPoint - ebmlReader.ElementPosition);
                ebmlReader.EnterContainer();

                long cueClusterPos = FindPosition(memoryStream, CUECLUSTERPOSITION);
                ebmlReader.ReadAt(0);
                ulong clusterPos = ebmlReader.ReadUInt();

                clusterPositions.Add(new ClusterPosition(timeStamp, clusterPos + posToAdd));

                ebmlReader.LeaveContainer();
                ebmlReader.LeaveContainer();
                memoryStream.Seek(startPos + cuePoint + cuePointSize + 2, SeekOrigin.Begin); //the 2 is because it has the identifier length and the size length,  each one of 1 byte size
                cuePoint = FindPosition(memoryStream, CUEPOINT) - startPos;
                ebmlReader.ReadAt(cuePoint);


            }

        }

        /// <summary>
        /// Extrae el formato de audio de opus
        /// </summary>
        /// <param name="memoryStream"></param>
        /// <returns>Formato de audio</returns>
        private static OpusFormat GetOpusFormat(MemoryStream memoryStream)
        {

            OpusFormat? opusFormat = null;
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
                    //ebmlReader.ReadAt(0);
                    //ulong trackNumber = ebmlReader.ReadUInt();

                    //var startPos = ebmlReader.ElementPosition;
                    //ebmlReader.ReadAt(memoryStream.Position-startPos);

                    //ulong trackUID = ebmlReader.ReadUInt();
                    //ebmlReader.ReadAt(memoryStream.Position - startPos);

                    //ulong trackType = ebmlReader.ReadUInt();
                    //ebmlReader.ReadAt(memoryStream.Position - startPos);

                    //ulong flagLacing = ebmlReader.ReadUInt();
                    //ebmlReader.ReadAt(memoryStream.Position - startPos);

                    long posCodec = FindPosition(memoryStream, CODECID);
                    ebmlReader.ReadAt(0);
                    codec = ebmlReader.ReadUtf();
                    isFound = codec == CODECNAME;
                }
                catch
                {
                    isFound = false;
                    memoryStream.Seek(posTracks + 1, SeekOrigin.Begin);
                };
            }
            return isFound;
        }
        /// <summary>
        ///  packets for clusters
        /// </summary>
        /// <param name="clusters">List of clusters that will be filled with packets</param>
        /// <returns></returns>
        public async Task<List<Cluster>> GetClusters()
        {
            List<Cluster> clusters = new List<Cluster>();
            await DownloadClusterPositions();
            foreach(var clusterPos in clusterPositions)
               clusters.Add(await DownloadCluster(clusterPos));
            HasFinished = true;
            OnFinished?.Invoke(this);
            return clusters;
        }
        /// <summary>
        /// Start Downloading all the content and calling the events
        /// </summary>
        /// <returns></returns>
        public async Task GetPackets()
        {
            await DownloadClusterPositions();
            foreach (var clusterPos in clusterPositions)
                await DownloadCluster(clusterPos);
            HasFinished = true;
            OnFinished?.Invoke(this);
        }

        public async Task DownloadClusterPositions()
        {
            byte[] buffer = await ytStream.DownloadClusterPositions();
            MemoryStream auxStream = new MemoryStream(buffer);
            ulong posToAdd = GetSeekHead(auxStream);
            if (IsSupportedCodec(auxStream))
            {
                OpusFormat opusFormat = GetOpusFormat(auxStream);
                OpusFormat = opusFormat;
                GetClusterPositions(auxStream, posToAdd);
            }
        }
        public async Task<Cluster> DownloadCluster(ClusterPosition clusterPosition)
        {
            
            List<OpusPacket> opusPacketsCluster = new List<OpusPacket>();
            long posCluster = (long)clusterPosition.ClusterPos;
            int nextClusterIndex = clusterPositions.IndexOf(clusterPosition) + 1;
            long nextPos = 0;

            if (nextClusterIndex < clusterPositions.Count)
                nextPos = (long)clusterPositions[nextClusterIndex].ClusterPos;
            else
                nextPos = ytStream.Capacity;

            byte[] buffer = await ytStream.DownloadCluster((ulong)posCluster, (ulong)nextPos);
                
            MemoryStream auxStream = new MemoryStream(buffer);
            //checkea si ya se ha procesado el cluster
            if (posCluster != ERRORCODE)
            {
                EbmlReader ebmlReader = new EbmlReader(auxStream);
                long clusterSize = EnterContainer(ebmlReader, 0);
                var startPos = auxStream.Position;
                //---DESCARTA EL TIMESTAMP DEL PRINCIPIO---
                ebmlReader.ReadAt(0);
                //---SE COGE EL TIME DESDE CLUSTERPOSTITIONS---
                var time = ebmlReader.ReadUInt();
                long nextClusterPos = clusterSize + posCluster;

                //-------------POSBLOCK ES RELATIVO A LA POSICION DEL PRIMER BLOQUE----------------


                long posBlock = FindPosition(auxStream, SIMPLEBLOCK) - startPos;
                OpusDecoder opusDecoder = new OpusDecoder((int)OpusFormat.sampleFrequency, OpusFormat.channels);
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
                        catch { isError = true; }

                        if (!isError)
                        {
                            try
                            {
                                OpusPacket opusPacket = GetBuffer(ebmlReader, auxStream, (int)clusterPositions[currentDecodingIndex].TimeStamp);
                                opusPacketsCluster.Add(opusPacket);
                                OnPacketDownloaded?.Invoke(this, opusPacket);
                            }
                            catch (Exception e) { ebmlReader.LeaveContainer(); auxStream.Seek(nextClusterPos, SeekOrigin.Begin); }
                        }
                        //---------------COMO LA POSICION DEL BLOQUE ES RELATIVA AL PRIMER BLOQUE SE RESTA LA POSICION--------
                        posBlock = FindPosition(auxStream, SIMPLEBLOCK) - startPos;
                        //---------------SI POSBLOCK ES NEGATIVO ENTONCES NO HA ENCONTRADO SIGUENTE BLOQUE------------
                        isError = posBlock < 0;

                    }

                }
                //------------------RESETEA LA POSICION AL BLOQUE Y SALE DEL CONTENEDOR--------------------

                ebmlReader.LeaveContainer();


            }
            Cluster cluster = new Cluster(opusPacketsCluster, clusterPosition.TimeStamp);
            clusterPosition.IsClusterDownloaded = true;
            OnClusterDownloaded?.Invoke(this,cluster);
            return cluster;
            
        }
        /// <summary>
        /// Decodifica los paquetes opus
        /// </summary>
        /// <param name="opusPackets"></param>
        /// <returns>Array de bytes decodificado</returns>
        public static byte[] GetPcm(List<OpusPacket> opusPackets, OpusFormat opusFormat)
        {
            OpusDecoder opusDecoder = new OpusDecoder((int)opusFormat.sampleFrequency, opusFormat.channels);
            List<byte> pcm = new List<byte>();

            foreach (var opus in opusPackets)
            {
                try
                {
                    OpusPacket opusPacket = opus;
                    short[] pcmBuffer = new short[opusPacket.ChannelCount * opusPacket.Frames * opusPacket.FrameSize];
                    int decodedSamples = opusDecoder.Decode(opusPacket.OpusBuffer, 0, opusPacket.OpusBuffer.Length, pcmBuffer, 0, opusPacket.FrameSize);
                    byte[] pcmBufferInBytes = new byte[pcmBuffer.Length * 2];
                    Buffer.BlockCopy(pcmBuffer, 0, pcmBufferInBytes, 0, pcmBufferInBytes.Length);
                    pcm.AddRange(pcmBufferInBytes);
                }

                catch { }
            }

            return pcm.ToArray();
        }
        public static byte[] GetPcm(OpusPacket opusPacket, OpusFormat opusFormat)
        {
            OpusDecoder opusDecoder = new OpusDecoder((int)opusFormat.sampleFrequency, opusFormat.channels);
            byte[] pcm = new byte[0];


            try
            {
                short[] pcmBuffer = new short[opusPacket.ChannelCount * opusPacket.Frames * opusPacket.FrameSize];
                int decodedSamples = opusDecoder.Decode(opusPacket.OpusBuffer, 0, opusPacket.OpusBuffer.Length, pcmBuffer, 0, opusPacket.FrameSize);
                byte[] pcmBufferInBytes = new byte[pcmBuffer.Length * 2];
                Buffer.BlockCopy(pcmBuffer, 0, pcmBufferInBytes, 0, pcmBufferInBytes.Length);
                pcm = pcmBufferInBytes;
            }

            catch { }

            return pcm;

        }

        /// <summary>
        /// Get the cluster position
        /// </summary>
        /// <param name="timeSpan">timespan for cluster</param>
        /// <returns>the cluster position or null if not found</returns>
        public ClusterPosition GetClusterPositionForTimeSpan(int timeSpan) => clusterPositions.LastOrDefault(i => i.TimeStamp <= (ulong)timeSpan);

        /// <summary>
        /// Extrae los datos del bloque
        /// </summary>
        /// <param name="ebmlReader"></param>
        /// <param name="auxStream"></param>
        /// <returns>array de bytes con los datos del bloque</returns>
        private static OpusPacket GetBuffer(EbmlReader ebmlReader, Stream auxStream, int relativeTime)
        {
            byte[] opusBuffer = new byte[ebmlReader.ElementSize - 4];
            byte[] timeSpan = new byte[2]; // signed int16
            auxStream.Seek(auxStream.Position + 1, SeekOrigin.Begin); //seek a timespan
            ebmlReader.ReadBinary(timeSpan, 0, timeSpan.Length);
            if (BitConverter.IsLittleEndian)
                Array.Reverse(timeSpan);
            int timeSpanInMs = (BitConverter.ToInt16(timeSpan, 0) + relativeTime);
            auxStream.Seek(auxStream.Position + 1, SeekOrigin.Begin); //omite los flags
            ebmlReader.ReadBinary(opusBuffer, 0, opusBuffer.Length);
            return new OpusPacket(opusBuffer, timeSpanInMs);
        }
        /// <summary>
        /// Espera hasta que se descargue cierta cantidad de bytes
        /// </summary>
        /// <param name="songStream">stream que esta descargando los datos</param>
        /// <param name="bytes">bytes hasta los que esperar</param>
        private void WaitForDownloadedBytes(long bytes)
        {
            while (ytStream.DownloadedBytes < bytes) { }

        }
        /// <summary>
        /// Entra al cluster
        /// </summary>
        /// <param name="ebmlReader"></param>
        /// <param name="posCluster">Posicion del cluster</param>
        /// <returns></returns>
        private static long EnterContainer(EbmlReader ebmlReader, long posCluster)
        {
            ebmlReader.ReadAt(posCluster);
            long clusterSize = ebmlReader.ElementSize;
            ebmlReader.EnterContainer();
            return clusterSize;
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
