using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record Vector3Dto(
    [property: JsonPropertyName("x")] float X,
    [property: JsonPropertyName("y")] float Y,
    [property: JsonPropertyName("z")] float Z
);
