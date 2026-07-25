using Jomolith.Towers.Domain.Enums;

namespace Jomolith.Towers.Domain.Properties;

public record VisualProperties(
    float Opacity = 1.0f,
    string ColourHex = "#A3A2A5",
    SurfaceType SurfaceXp = SurfaceType.Smooth,
    SurfaceType SurfaceXn = SurfaceType.Smooth,
    SurfaceType SurfaceYp = SurfaceType.Smooth,
    SurfaceType SurfaceYn = SurfaceType.Smooth,
    SurfaceType SurfaceZp = SurfaceType.Smooth,
    SurfaceType SurfaceZn = SurfaceType.Smooth
);
