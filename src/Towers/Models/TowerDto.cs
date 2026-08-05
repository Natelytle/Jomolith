using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record TowerDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("creator")] string Creator,
    [property: JsonPropertyName("difficulty")] double Difficulty,
    [property: JsonPropertyName("spawn_position")] Vector3Dto SpawnPosition,
    [property: JsonPropertyName("parts")] List<PartDto> Parts,
    [property: JsonPropertyName("client_objects")] List<ClientObjectDto> ClientObjects
);
