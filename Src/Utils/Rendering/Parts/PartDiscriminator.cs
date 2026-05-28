using Godot;
using Jomolith.Utils.Rendering.MeshHeapSystem;

namespace Jomolith.Utils.Rendering.Parts;

public struct PartDiscriminator : IHeapDiscriminator<PartDiscriminator>
{
    public bool Matches(PartDiscriminator other)
    {
        return true;
    }

    public void SetupBlock(MultiMeshInstance3D mm)
    {
    }
}
