# Youpus
Simple Webm opus demuxer for youtube videos

## Whats this for?
This is basiclly a demuxer and decoder for the webm youtube videos.
## Does it work?
I think so :V.
## What can be done with this?
Weeeell,I started this because i was trying to make a music discord bot.
But I think it can be used for making a youtube music player, converter...

### Example
```cs
YtStream stream = new YtStream(YtStream.GetSongUrl(args[0]).Result);
WebmToOpus opus = new WebmToOpus(stream);
List<OpusPacket> opusPackets = new List<OpusPacket>();
opus.DownloadClusterPositions().Wait();

foreach(var clusterPos in opus.ClusterPositions)
{
    var cluster = opus.DownloadCluster(clusterPos).Result;
    opusPackets.AddRange(cluster.Packets);
}
var c = opus.GetClusterPositionForTimeSpan(569);

byte[] pcmBufferBytes = WebmToOpus.GetPcm(opusPackets, opus.OpusFormat);
MemoryStream memoryStream = new MemoryStream(pcmBufferBytes);
var rawSourceWaveStream = new RawSourceWaveStream(pcmBufferBytes, 0, pcmBufferBytes.Length, new WaveFormat((int)opus.OpusFormat.sampleFrequency, opus.OpusFormat.channels));
WaveFileWriter.CreateWaveFile("output.wav", rawSourceWaveStream);
```
This is the first time I work with webm and ebml files so the code can be messy. 

### Libraries used
- [Ebml-reaader](https://github.com/matthewn4444/EBMLReader)
- [Concentus](https://github.com/lostromb/concentus)
- [YoutubeExplode](https://github.com/Tyrrrz/YoutubeExplode)