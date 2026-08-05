using Godot;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Factory.Shapes;

public class WedgeShapeBuilder : IShapeBuilder
{
    public Mesh BuildMesh(PartModel part) => new PrismMesh
    {
        LeftToRight = 0f,
        Size = new Vector3(part.Scale.Z, part.Scale.Y, part.Scale.X)
    };

    public Shape3D BuildCollisionShape(PartModel part)
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
}
