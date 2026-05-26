using Godot;
using Jomolith.Core.Objects;

namespace Jomolith.Utils.Towers.Objects;

public partial class BallPart : Part
{
    protected override MeshInstance3D GetPartMesh(PartModel partModel) => new MeshInstance3D
    {
        Mesh = new SphereMesh
        {
            Radius = partModel.SphereRadius,
            Height = partModel.SphereRadius * 2.0f
        }
    };

    protected override CollisionShape3D GetPartCollisionShape(PartModel partModel) => new CollisionShape3D
    {
        Shape = new SphereShape3D
        {
            Radius = partModel.SphereRadius
        }
    };
}
