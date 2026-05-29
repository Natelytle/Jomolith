using System.Numerics;
using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Serialization;

public struct SerializableQuaternion
{
    [JsonPropertyName("x")] public float X { get; set; } = 0.0f;
    [JsonPropertyName("y")] public float Y { get; set; } = 0.0f;
    [JsonPropertyName("z")] public float Z { get; set; } = 0.0f;
    [JsonPropertyName("w")] public float W { get; set; } = 1.0f;

    public SerializableQuaternion()
    {
    }

    public static implicit operator SerializableQuaternion(Quaternion q) =>
        new SerializableQuaternion { X = q.X, Y = q.Y, Z = q.Z, W = q.W };

    public static implicit operator Quaternion(SerializableQuaternion q) => new Quaternion(q.X, q.Y, q.Z, q.W);
}
