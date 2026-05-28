
using Godot;
using Jomolith.Core.Objects;
using Jomolith.Utils.Rendering.Parts;

namespace Jomolith.Utils.Towers.Objects;

public abstract partial class Part : RigidBody3D
{
    public Transform3D MeshTransform => GlobalTransform.ScaledLocal(scale);
    private Vector3 scale;

    public Color PartColour { get; private set; }
    public int Shape { get; private set; }
    public int Material { get; private set; }

    protected PartHeapUser HeapUser { get; private set; } = null!;

    private byte surfaceVariantXp = 1;
    private byte surfaceVariantXn = 1;
    private byte surfaceVariantYp = 1;
    private byte surfaceVariantYn = 1;
    private byte surfaceVariantZp = 1;
    private byte surfaceVariantZn = 1;

    // Used as a custom parameter in the MultiMeshInstance3D
    public int PackedSurfaceVariants => (
        (surfaceVariantXp) |
        (surfaceVariantXn << 5) |
        (surfaceVariantYp << 10) |
        (surfaceVariantYn << 15) |
        (surfaceVariantZp << 20) |
        (surfaceVariantZn << 25)
    );

    public void Initialize(PartModel partModel)
    {
        // Set up the initial rigidbody and set the freeze mode if anchored.
        Position = new Vector3(partModel.Position.X, partModel.Position.Y, partModel.Position.Z);
        Rotation = new Quaternion(partModel.Rotation.X, partModel.Rotation.Y, partModel.Rotation.Z, partModel.Rotation.W).Normalized().GetEuler();
        Freeze = partModel.Anchored;

        scale = GetMeshScale(partModel);
        PartColour = new Color(partModel.VisualProperties.Colour.R, partModel.VisualProperties.Colour.G, partModel.VisualProperties.Colour.B, partModel.VisualProperties.Opacity);
        Shape = (int)partModel.Shape;

        surfaceVariantXp = (byte)partModel.VisualProperties.SurfaceTypeXp;
        surfaceVariantXn = (byte)partModel.VisualProperties.SurfaceTypeXn;
        surfaceVariantYp = (byte)partModel.VisualProperties.SurfaceTypeYp;
        surfaceVariantYn = (byte)partModel.VisualProperties.SurfaceTypeYn;
        surfaceVariantZp = (byte)partModel.VisualProperties.SurfaceTypeZp;
        surfaceVariantZn = (byte)partModel.VisualProperties.SurfaceTypeZn;

        Material = partModel.VisualProperties.Opacity < 0.99f ? 1 : 0;

        HeapUser = new PartHeapUser(this);

        if (partModel.CanCollide)
        {
            CollisionShape3D collider = GetPartCollisionShape(partModel);

            AddChild(collider);

            SetCollisionLayerValue(1, true);
            SetCollisionLayerValue(2, true);
        }
    }

    internal void AddToPartHeap(PartHeap heap) {
        heap.AppendInstance(HeapUser, Shape, 0);
    }

    internal void RemoveFromPartHeap() {
        HeapUser.Remove();
    }

    protected abstract Vector3 GetMeshScale(PartModel partModel);

    protected abstract CollisionShape3D GetPartCollisionShape(PartModel partModel);
}
