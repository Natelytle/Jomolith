
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Jomolith.Tower.Core.Objects;

namespace Jomolith.Utils.Towers.Objects;

public abstract partial class Part : RigidBody3D
{
    public void Initialize(PartModel partModel)
    {
        // Set up the initial rigidbody and set the freeze mode if anchored.
        Position = new Vector3(partModel.Position.X, partModel.Position.Y, partModel.Position.Z);
        Rotation = new Quaternion(partModel.Rotation.X, partModel.Rotation.Y, partModel.Rotation.Z, partModel.Rotation.W).Normalized().GetEuler();
        Freeze = partModel.Anchored;

        if (partModel.VisualProperties.Opacity > 0)
        {
            MeshInstance3D mesh = GetPartMesh(partModel);

            StandardMaterial3D material = new StandardMaterial3D
            {
                AlbedoColor = new Color(partModel.VisualProperties.Colour.R, partModel.VisualProperties.Colour.G,
                    partModel.VisualProperties.Colour.B, partModel.VisualProperties.Opacity)
            };

            if (partModel.VisualProperties.Opacity < 0.99f)
            {
                material.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
                material.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.Always;
            }

            mesh.SetSurfaceOverrideMaterial(0, material);

            mesh.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

            AddChild(mesh);
        }

        if (partModel.CanCollide)
        {
            CollisionShape3D collider = GetPartCollisionShape(partModel);

            AddChild(collider);

            SetCollisionLayerValue(1, true);
            SetCollisionLayerValue(2, true);
        }
    }

    protected abstract MeshInstance3D GetPartMesh(PartModel partModel);

    protected abstract CollisionShape3D GetPartCollisionShape(PartModel partModel);
}
