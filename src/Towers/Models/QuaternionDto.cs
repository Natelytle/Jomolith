using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record QuaternionDto(
    [property: JsonPropertyName("x")] float X,
    [property: JsonPropertyName("y")] float Y,
    [property: JsonPropertyName("z")] float Z,
    [property: JsonPropertyName("w")] float W
);
