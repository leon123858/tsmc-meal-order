using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace mail.utils;

public class ByteArrayJsonConverter : JsonConverter<byte[]>
{
    public override byte[] Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var sByteArray = JsonSerializer.Deserialize<short[]>(ref reader);
        var value = new byte[sByteArray.Length];
        for (var i = 0; i < sByteArray.Length; i++)
        {
            value[i] = (byte)sByteArray[i];
        }

        return value;
    }

    public override void Write(Utf8JsonWriter writer, byte[] value, JsonSerializerOptions options)
    {
        writer.WriteStartArray();

        foreach (var val in value)
        {
            writer.WriteNumberValue(val);
        }

        writer.WriteEndArray();
    }
}