using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record VisualPropertiesDto(
    [property: JsonPropertyName("opacity")] float Opacity,
    [property: JsonPropertyName("colour")] string ColourHex,
    [property: JsonPropertyName("surface_type_xp")] string SurfaceXp,
    [property: JsonPropertyName("surface_type_xn")] string SurfaceXn,
    [property: JsonPropertyName("surface_type_yp")] string SurfaceYp,
    [property: JsonPropertyName("surface_type_yn")] string SurfaceYn,
    [property: JsonPropertyName("surface_type_zp")] string SurfaceZp,
    [property: JsonPropertyName("surface_type_zn")] string SurfaceZn
);
