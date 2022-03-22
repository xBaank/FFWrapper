using System.Collections.Generic;
using System.Text.Json.Serialization;

using FFmpegWrapper.JsonModels;

public class Packet
{
    [JsonPropertyName("codec_type")]
    public string? CodecType { get; set; }

    [JsonPropertyName("stream_index")]
    public int? StreamIndex { get; set; }

    [JsonPropertyName("pts")]
    public int? Pts { get; set; }

    [JsonPropertyName("pts_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? PtsTime { get; set; }

    [JsonPropertyName("dts")]
    public int? Dts { get; set; }

    [JsonPropertyName("dts_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? DtsTime { get; set; }

    [JsonPropertyName("duration")]
    public int? Duration { get; set; }

    [JsonPropertyName("duration_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? DurationTime { get; set; }

    [JsonPropertyName("size")]
    [JsonConverter(typeof(LongConverter))]
    public long? Size { get; set; }

    [JsonPropertyName("pos")]
    [JsonConverter(typeof(LongConverter))]
    public long? Pos { get; set; }

    [JsonPropertyName("flags")]
    public string? Flags { get; set; }
}

