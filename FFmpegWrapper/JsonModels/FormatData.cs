using System.Text.Json.Serialization;

namespace FFmpegWrapper.JsonModels
{
    public class Tags
    {
        [JsonPropertyName("encoder")]
        public string? Encoder { get; set; }
    }

    public class FormatMetadata
    {
        [JsonPropertyName("filename")]
        public string? Filename { get; set; }

        [JsonPropertyName("nb_streams")]
        public int? NbStreams { get; set; }

        [JsonPropertyName("nb_programs")]
        public int? NbPrograms { get; set; }

        [JsonPropertyName("format_name")]
        public string? FormatName { get; set; }

        [JsonPropertyName("format_long_name")]
        public string? FormatLongName { get; set; }

        [JsonPropertyName("start_time")]
        [JsonConverter(typeof(DoubleConverter))]
        public double? StartTime { get; set; }

        [JsonPropertyName("duration")]
        [JsonConverter(typeof(DoubleConverter))]
        public double? Duration { get; set; }

        [JsonPropertyName("size")]
        [JsonConverter(typeof(LongConverter))]
        public long? Size { get; set; }

        [JsonPropertyName("bit_rate")]
        [JsonConverter(typeof(LongConverter))]
        public long? BitRate { get; set; }

        [JsonPropertyName("probe_score")]
        public int? ProbeScore { get; set; }

        [JsonPropertyName("tags")]
        public Tags? Tags { get; set; }
    }

    public class FormatRoot
    {
        [JsonPropertyName("format")]
        public FormatMetadata? Format { get; set; }
    }


}


