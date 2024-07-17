using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

public class JsonConverterDateTime : JsonConverter<DateTime>
{
    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return DateTime.ParseExact(reader.GetString(), "dd-MMM-yyyy", CultureInfo.CurrentCulture);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString("dd-MMM-yyyy"));
    }
}
