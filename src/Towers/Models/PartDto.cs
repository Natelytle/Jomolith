using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record PartDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("type")] string Type,
    [property: JsonPropertyName("shape")] string Shape,
    [property: JsonPropertyName("position")] Vector3Dto Position,
    [property: JsonPropertyName("rotation")] QuaternionDto Rotation,
    [property: JsonPropertyName("scale")] Vector3Dto Scale,
    [property: JsonPropertyName("can_collide")] bool CanCollide,
    [property: JsonPropertyName("anchored")] bool Anchored,
    [property: JsonPropertyName("physical_properties")] PhysicalPropertiesDto PhysicalProperties,
    [property: JsonPropertyName("visual_properties")] VisualPropertiesDto VisualProperties,
    [property: JsonPropertyName("children")] List<PartDto> Children
);
