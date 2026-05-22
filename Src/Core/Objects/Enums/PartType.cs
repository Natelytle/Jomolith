using System.Text.Json.Serialization;

namespace Jomolith.Core.Objects.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PartType
{
    Block,
    Ball,
    Cylinder,
    Wedge,
    CornerWedge
}