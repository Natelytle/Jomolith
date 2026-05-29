using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Dto;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(PartDto), "part")]
public abstract class TowerObjectDto
{
    [JsonPropertyName("name")] public string Name { get; init; } = null!;

    [JsonPropertyName("children")] public List<TowerObjectDto> Children { get; init; } = new List<TowerObjectDto>();
}
