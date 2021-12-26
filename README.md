# Youpus
Simple Webm opus demuxer for youtube videos

## Whats this for?
This is basically a demuxer and decoder for the webm youtube videos.
## What can be done with this?
It can demuxe webm files.

### Example
```cs
YtStream stream = new YtStream(await YtStream.GetSongUrl("https://www.youtube.com/watch?v=E3Huy2cdih0"));
WebmToOpus opus = new WebmToOpus(stream);
List<OpusPacket> opusPackets = new List<OpusPacket>();
//Download clusters positions
await opus.DownloadClusterPositions();
//Donwload all clusters
foreach(var clusterPos in opus.ClusterPositions)
{
    var cluster = await opus.DownloadCluster(clusterPos);
    opusPackets.AddRange(cluster.Packets);
}
//Decodes to pcm
byte[] pcmBufferBytes = WebmToOpus.GetPcm(opusPackets, opus.OpusFormat);
//Writes pcm to wav file
MemoryStream memoryStream = new MemoryStream(pcmBufferBytes);
var rawSourceWaveStream = new RawSourceWaveStream(pcmBufferBytes, 0, pcmBufferBytes.Length, new WaveFormat((int)opus.OpusFormat.sampleFrequency, opus.OpusFormat.channels));
WaveFileWriter.CreateWaveFile("output.wav", rawSourceWaveStream);
```
# TODO
- [X] Audio track
- [ ] Video track
- [ ] Blocks and Block Groups
- [X] Simpleblocks
- [ ] Multiple tracks
- [ ] Local files


### Libraries used
- [EbmlReader](https://github.com/matthewn4444/EBMLReader)
- [Concentus](https://github.com/lostromb/concentus)
- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)
