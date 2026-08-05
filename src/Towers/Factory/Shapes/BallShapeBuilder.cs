using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class BallShapeBuilder : IShapeBuilder
{
    public Mesh BuildMesh(PartModel part) => new SphereMesh
    {
        Radius = part.SphereRadius,
        Height = part.SphereRadius * 2.0f
    };

    public Shape3D BuildCollisionShape(PartModel part) => new SphereShape3D
    {
        Radius = part.SphereRadius
    };

    public float GetVolume(PartModel part) => 4 / 3.0f * float.Pi * part.SphereRadius * part.SphereRadius * part.SphereRadius;
}
