using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record PhysicalPropertiesDto(
    [property: JsonPropertyName("density")] float Density,
    [property: JsonPropertyName("friction")] float Friction,
    [property: JsonPropertyName("elasticity")] float Elasticity
);
