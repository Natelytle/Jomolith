using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class WedgeShapeBuilder : ShapeBuilder
{
    public override Mesh BuildMesh(PartModel part) => BuildWedgeMesh(false);

    public override Vector3 MeshScale(PartModel part) => new(part.Scale.X, part.Scale.Y, part.Scale.Z);

    public override Shape3D BuildCollisionShape(PartModel part)
    {
        // X and Z are flipped because in roblox, the wedge shape's slope is dictated by Z, but in godot it is dictated by X.
        // We rotate the whole part around the Y axis afterward to fix this.
        float x = part.Scale.Z / 2;
        float y = part.Scale.Y / 2;
        float z = part.Scale.X / 2;

        return new ConvexPolygonShape3D
        {
            Points = new[]
            {
                new Vector3(-x, -y, -z),
                new Vector3(-x, -y, z),
                new Vector3(x, -y, -z),
                new Vector3(x, -y, z),
                new Vector3(-x, y, -z),
                new Vector3(-x, y, z)
            }
        };
    }

    public override float GetVolume(PartModel part) => part.Scale.X * part.Scale.Y * part.Scale.Z / 2.0f;
}
