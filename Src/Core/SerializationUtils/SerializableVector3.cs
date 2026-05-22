using System.Numerics;
using System.Text.Json.Serialization;

namespace Jomolith.Core.SerializationUtils;

public struct SerializableVector3
{
    [JsonPropertyName("x")] public float X { get; set; }
    [JsonPropertyName("y")] public float Y { get; set; }
    [JsonPropertyName("z")] public float Z { get; set; }

    public static implicit operator SerializableVector3(Vector3 v) =>
        new SerializableVector3 { X = v.X, Y = v.Y, Z = v.Z };

    public static implicit operator Vector3(SerializableVector3 v) => new Vector3(v.X, v.Y, v.Z);
}
