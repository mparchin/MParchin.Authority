using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace MParchin.Authority.JsonConverter;

public class DateTimeToEpochConverter : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        new(DateTimeOffset.FromUnixTimeMilliseconds(reader.GetInt64()).DateTime.Ticks, DateTimeKind.Utc);

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteNumberValue(Convert.ToInt64((value.ToUniversalTime() - new DateTime(1970, 1, 1)).TotalMilliseconds));
}