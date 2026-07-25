namespace Jomolith.Towers.Domain.Properties;

public record PhysicalProperties(
    float Density = 1.0f,
    float Friction = 1.0f,
    float Elasticity = 0.5f
);
