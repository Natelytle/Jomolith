using System;
using Godot;

namespace Jomolith.Utils.Rendering.MeshHeapSystem;

public interface IHeapDiscriminator<in TSelf>
where TSelf : struct, IHeapDiscriminator<TSelf>
{
    public abstract bool Matches(TSelf other);
    public abstract void SetupBlock(MultiMeshInstance3D mm);

}
