
using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class BallShapeBuilder : ShapeBuilder
{
    public override Mesh BuildMesh(PartModel part) => BuildBlockMesh(ROUND_SEGMENTS, ROUND_SEGMENTS, true, true, false);

    public override Vector3 MeshScale(PartModel part) =>
        new(part.SphereRadius * 2f, part.SphereRadius * 2f, part.SphereRadius * 2f);

    public override Shape3D BuildCollisionShape(PartModel part) => new SphereShape3D
    {
        Radius = part.SphereRadius
    };

    public override float GetVolume(PartModel part) => 4 / 3.0f * float.Pi * part.SphereRadius * part.SphereRadius * part.SphereRadius;
}
