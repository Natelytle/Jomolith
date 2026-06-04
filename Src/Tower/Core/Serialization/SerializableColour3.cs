using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Serialization;

[JsonConverter(typeof(HexColourConverter))]
public record struct SerializableColour3
{
    [JsonPropertyName("r")] public float R { get; set; }
    [JsonPropertyName("g")] public float G { get; set; }
    [JsonPropertyName("b")] public float B { get; set; }

    public SerializableColour3(float r = 0, float g = 0, float b = 0)
    {
        R = r;
        G = g;
        B = b;
    }
}

public class HexColourConverter : JsonConverter<SerializableColour3>
{
    public override SerializableColour3 Read(ref Utf8JsonReader reader, Type typeToConvert,
        JsonSerializerOptions options)
    {
        string hex = reader.GetString()!.TrimStart('#');

        float r = Convert.ToInt32(hex[0..2], 16) / 255f;
        float g = Convert.ToInt32(hex[2..4], 16) / 255f;
        float b = Convert.ToInt32(hex[4..6], 16) / 255f;

        return new SerializableColour3(r, g, b);
    }

    public override void Write(Utf8JsonWriter writer, SerializableColour3 value, JsonSerializerOptions options)
    {
        int r = (int)(value.R * 255);
        int g = (int)(value.G * 255);
        int b = (int)(value.B * 255);

        writer.WriteStringValue($"#{r:X2}{g:X2}{b:X2}");
    }
}
