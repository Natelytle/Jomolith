using Godot;
using Jomolith.Core.Objects;

namespace Jomolith.Utils.Towers.Objects;

public partial class BlockPart : Part
{
    protected override MeshInstance3D GetPartMesh(PartModel partModel) => new MeshInstance3D
    {
        Mesh = new BoxMesh
        {
            Size = new Vector3(partModel.Scale.X, partModel.Scale.Y, partModel.Scale.Z)
        }
    };

    protected override CollisionShape3D GetPartCollisionShape(PartModel partModel) => new CollisionShape3D
    {
        Shape = new BoxShape3D
        {
            Size = new Vector3(partModel.Scale.X, partModel.Scale.Y, partModel.Scale.Z)
        },
    };
}
