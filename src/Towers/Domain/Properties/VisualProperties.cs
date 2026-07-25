namespace Jomolith.Towers.Domain.Properties;

public record VisualProperties(
    float Opacity = 1.0f,
    string ColourHex = "#A3A2A5",
    string SurfaceXp = "smooth",
    string SurfaceXn = "smooth",
    string SurfaceYp = "smooth",
    string SurfaceYn = "smooth",
    string SurfaceZp = "smooth",
    string SurfaceZn = "smooth"
);
