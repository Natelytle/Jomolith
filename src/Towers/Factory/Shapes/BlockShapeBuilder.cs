using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class BlockShapeBuilder : IShapeBuilder
{
    public Mesh BuildMesh(PartModel part) => new BoxMesh
    {
        Size = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z)
    };

    public Shape3D BuildCollisionShape(PartModel part) => new BoxShape3D
    {
        Size = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z)
    };
}
