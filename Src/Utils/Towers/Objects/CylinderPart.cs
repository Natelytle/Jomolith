using Godot;
using Jomolith.Core.Objects;

namespace Jomolith.Utils.Towers.Objects;

public partial class CylinderPart : Part
{
    protected override Vector3 GetMeshScale(PartModel partModel) =>
        new Vector3(partModel.CylinderRadius, partModel.Scale.Y, partModel.CylinderRadius);

    protected override CollisionShape3D GetPartCollisionShape(PartModel partModel) => new CollisionShape3D
    {
        Shape = new CylinderShape3D
        {
            Height = partModel.Height,
            Radius = partModel.CylinderRadius
        }
    };
}
