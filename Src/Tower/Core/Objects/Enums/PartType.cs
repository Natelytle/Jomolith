using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Objects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PartType
{
    Block,
    Ball,
    Cylinder,
    Wedge,
    CornerWedge
}
