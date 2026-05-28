using System.Runtime.CompilerServices;
using Godot;
using Jomolith.Core.Objects;
using Jomolith.Utils.Rendering.MeshHeapSystem;
using Jomolith.Utils.Towers.Objects;

namespace Jomolith.Utils.Rendering.Parts;

public class PartHeapUser : MeshHeapUser<PartHeapUser, PartDiscriminator>
{
    private Part part;

    public PartHeapUser(Part part)
    {
        this.part = part;
    }

    public override PartDiscriminator HeapDiscriminator => new PartDiscriminator();
    public override Transform3D HeapTransform => part.MeshTransform;
    public override Color HeapColor => part.PartColour;

    public override Color HeapCustomData
    {
        get
        {
            int surfaceVariants = part.PackedSurfaceVariants;
            float surfaceVariantsFloat = Unsafe.As<int, float>(ref surfaceVariants);
            return new Color(surfaceVariantsFloat, 0f, 0f, 0f);
        }
    }
}
