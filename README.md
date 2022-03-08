# FFWrapper
This is a horrible ffmpeg and ffprobe wrapper that I've made for fun.

## What can be done with this?
You can convert or fetch metadata from video/audio files using stream class or just the path.

### Convert to Stream
```cs
FFmpegClient fFmpegClient = new FFmpegClient();
Stream output = new MemoryStream();
await fFmpegClient.ConvertToStreamAsync(inputPath , output, new Format(FormatTypes.MP3));
```
### Convert to File
```cs
FFmpegClient fFmpegClient = new FFmpegClient();
await fFmpegClient.ConvertAsync(inputPath, outputPath);
```
### Get metadata
```cs
FFprobeClient fFprobeClient = new FFprobeClient();
var format = await fFprobeClient.GetMetadataAsync(inputPath);
```
### Get packets
```cs
FFprobeClient fFprobeClient = new FFprobeClient();
//You can say where the output will be stored, whether it is in the file system,memory...
//Returned value is already deserialized but you can still do whatever you want with the raw output data
var frames = await fFprobeClient.GetFramesAsync(inputPath, StreamType.a, new MemoryStream());
```
## About Windows and Unix
### Windows
The library itself tries to find the binaries on the same folder, but you can still use this constructor.
```cs
FFmpegClient fFmpegClient = new FFmpegClient("/bin/ffmpeg.exe");
```
### Unix
When using it on unix systems, it will directly use the ffmpeg/ffprobe command, or use the other constructor to specify the path.
