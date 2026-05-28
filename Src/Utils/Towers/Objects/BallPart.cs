using Godot;
using Jomolith.Core.Objects;

namespace Jomolith.Utils.Towers.Objects;

public partial class BallPart : Part
{
    protected override Vector3 GetMeshScale(PartModel partModel) =>
        new Vector3(partModel.SphereRadius, partModel.SphereRadius, partModel.SphereRadius);

    protected override CollisionShape3D GetPartCollisionShape(PartModel partModel) => new CollisionShape3D
    {
        Shape = new SphereShape3D
        {
            Radius = partModel.SphereRadius
        }
    };
}
