using System.Text.Json.Serialization;
using Jomolith.Core.Objects.Enums;
using Jomolith.Core.SerializationUtils;

namespace Jomolith.Core.Objects.Properties;

public class VisualProperties
{
    [JsonPropertyName("opacity")] public float Opacity { get; set; } = 1f;

    [JsonPropertyName("colour")]
    public SerializableColour3 Colour { get; set; } = new SerializableColour3(163, 162, 165);

    [JsonPropertyName("surface_type_xp")] public SurfaceType SurfaceTypeXp { get; set; } = SurfaceType.Studs;

    [JsonPropertyName("surface_type_xn")] public SurfaceType SurfaceTypeXn { get; set; } = SurfaceType.Studs;

    [JsonPropertyName("surface_type_yp")] public SurfaceType SurfaceTypeYp { get; set; } = SurfaceType.Studs;

    [JsonPropertyName("surface_type_yn")] public SurfaceType SurfaceTypeYn { get; set; } = SurfaceType.Studs;

    [JsonPropertyName("surface_type_zp")] public SurfaceType SurfaceTypeZp { get; set; } = SurfaceType.Studs;

    [JsonPropertyName("surface_type_zn")] public SurfaceType SurfaceTypeZn { get; set; } = SurfaceType.Studs;
}