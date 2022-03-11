using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Globalization;

namespace FFmpegWrapper.JsonModels
{
    public class LongConverter : JsonConverter<long>
    {
        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (long.TryParse(reader.GetString(), out long intVal))
            {
                return intVal;
            }
            // customize this part as necessary to satisfactorily deserialize
            // the float value as int, or throw exception, etc.
            float floatValue = reader.GetSingle();
            return (int)floatValue;
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    public class DoubleConverter : JsonConverter<double>
    {
        public override double Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (double.TryParse(reader.GetString(), NumberStyles.Float, new CultureInfo("en-US"), out double intVal))
            {
                return intVal;
            }
            // customize this part as necessary to satisfactorily deserialize
            // the float value as int, or throw exception, etc.
            float floatValue = reader.GetSingle();
            return (int)floatValue;
        }

        public override void Write(Utf8JsonWriter writer, double value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}