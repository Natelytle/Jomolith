using Godot;
using Jomolith.Core.Objects;

namespace Jomolith.Utils.Towers.Objects;

public partial class CylinderPart : Part
{
    protected override MeshInstance3D GetPartMesh(PartModel partModel) => new MeshInstance3D
    {
        Mesh = new CylinderMesh
        {
            BottomRadius = partModel.CylinderRadius,
            TopRadius = partModel.CylinderRadius,
            Height = partModel.Height
        }
    };

    protected override CollisionShape3D GetPartCollisionShape(PartModel partModel) => new CollisionShape3D
    {
        Shape = new CylinderShape3D
        {
            Height = partModel.Height,
            Radius = partModel.CylinderRadius
        }
    };
}
