
using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class CylinderShapeBuilder : ShapeBuilder
{
    public override Mesh BuildMesh(PartModel part) => BuildBlockMesh(2, ROUND_SEGMENTS, false, true, true);

    public override Vector3 MeshScale(PartModel part) =>
        new(part.Height, part.CylinderRadius * 2f, part.CylinderRadius * 2f);

    public override Shape3D BuildCollisionShape(PartModel part) => new CylinderShape3D
    {
        Height = part.Height,
        Radius = part.CylinderRadius
    };

    public override float GetVolume(PartModel part) => float.Pi * part.CylinderRadius * part.CylinderRadius * part.Height;
}
