using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class BlockShapeBuilder : ShapeBuilder
{
    public override Mesh BuildMesh(PartModel part) => BuildBlockMesh(2, 2, false, false, false);

    public override Vector3 MeshScale(PartModel part) => new(part.Scale.X, part.Scale.Y, part.Scale.Z);

    public override Shape3D BuildCollisionShape(PartModel part) => new BoxShape3D
    {
        Size = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z)
    };

    public override float GetVolume(PartModel part) => part.Scale.X * part.Scale.Y * part.Scale.Z;
}
