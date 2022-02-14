using System.Collections.Generic;
using System.Text.Json.Serialization;

public class Frame
{
    [JsonPropertyName("media_type")]
    public string? MediaType { get; set; }

    [JsonPropertyName("stream_index")]
    public int? StreamIndex { get; set; }

    [JsonPropertyName("key_frame")]
    public int? KeyFrame { get; set; }

    [JsonPropertyName("pkt_pts")]
    public int? PktPts { get; set; }

    [JsonPropertyName("pkt_pts_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? PktPtsTime { get; set; }

    [JsonPropertyName("pkt_dts")]
    public int? PktDts { get; set; }

    [JsonPropertyName("pkt_dts_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? PktDtsTime { get; set; }

    [JsonPropertyName("best_effort_timestamp")]
    public int? BestEffortTimestamp { get; set; }

    [JsonPropertyName("best_effort_timestamp_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? BestEffortTimestampTime { get; set; }

    [JsonPropertyName("pkt_duration")]
    public int? PktDuration { get; set; }

    [JsonPropertyName("pkt_duration_time")]
    [JsonConverter(typeof(DoubleConverter))]
    public double? PktDurationTime { get; set; }

    [JsonPropertyName("pkt_pos")]
    [JsonConverter(typeof(LongConverter))]
    public long? PktPos { get; set; }

    [JsonPropertyName("pkt_size")]
    [JsonConverter(typeof(LongConverter))]
    public long? PktSize { get; set; }

    [JsonPropertyName("sample_fmt")]
    public string? SampleFmt { get; set; }

    [JsonPropertyName("nb_samples")]
    public int? NbSamples { get; set; }

    [JsonPropertyName("channels")]
    public int? Channels { get; set; }

    [JsonPropertyName("channel_layout")]
    public string? ChannelLayout { get; set; }
}

public class FrameRoot
{
    [JsonPropertyName("frames")]
    public List<Frame>? Frames { get; set; }
}

