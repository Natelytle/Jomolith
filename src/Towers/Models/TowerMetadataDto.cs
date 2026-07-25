using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record TowerMetadataDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("difficulty")] int Difficulty,
    [property: JsonPropertyName("version")] int Version
);
