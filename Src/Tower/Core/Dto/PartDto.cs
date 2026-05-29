using System.Text.Json.Serialization;
using Jomolith.Tower.Core.Objects.Enums;
using Jomolith.Tower.Core.Objects.Properties;
using Jomolith.Tower.Core.Serialization;

namespace Jomolith.Tower.Core.Dto;

public class PartDto : TowerObjectDto
{
    [JsonPropertyName("shape")] public PartType Shape { get; set; }

    [JsonPropertyName("position")] public SerializableVector3 Position { get; set; }

    [JsonPropertyName("rotation")] public SerializableQuaternion Rotation { get; set; }

    [JsonPropertyName("scale")] public SerializableVector3 Scale { get; set; }

    [JsonPropertyName("can_collide")] public bool CanCollide { get; set; }

    [JsonPropertyName("anchored")] public bool Anchored { get; set; }

    [JsonPropertyName("physical_properties")]
    public PhysicalProperties PhysicalProperties { get; set; } = null!;

    [JsonPropertyName("visual_properties")]
    public VisualProperties VisualProperties { get; set; } = null!;
}
