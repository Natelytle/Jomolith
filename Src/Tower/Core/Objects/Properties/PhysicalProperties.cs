using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Objects.Properties;

public class PhysicalProperties
{
    [JsonPropertyName("density")] public float Density { get; set; } = 1f;

    [JsonPropertyName("friction")] public float Friction { get; set; } = 1f;

    [JsonPropertyName("elasticity")] public float Elasticity { get; set; } = 0.5f;
}
