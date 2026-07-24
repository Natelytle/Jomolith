using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class CylinderShapeBuilder : IShapeBuilder
{
    public Mesh BuildMesh(PartModel part) => new CylinderMesh
    {
        BottomRadius = part.CylinderRadius,
        TopRadius = part.CylinderRadius,
        Height = part.Height
    };

    public Shape3D BuildCollisionShape(PartModel part) => new CylinderShape3D
    {
        Height = part.Height,
        Radius = part.CylinderRadius
    };
}
