using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Game.Play.PlayerCamera;

public interface IPlayerCamera : INode3D
{
    /// <summary>
    ///     Used for smooth lerping of the camera position when zooming.
    /// </summary>
    Vector3 SpringArmTargetPosition { get; }

    /// <summary>Camera's local position within the camera system.</summary>
    Vector3 CameraLocalPosition { get; }

    /// <summary>Horizontal gimbal rotation in euler angles.</summary>
    Vector3 GimbalRotationHorizontal { get; }

    /// <summary>Vertical gimbal rotation in euler angles.</summary>
    Vector3 GimbalRotationVertical { get; }

    /// <summary>Camera's global transform basis.</summary>
    Basis CameraBasis { get; }

    /// <summary>
    ///     Local position of the offset node that is a parent of the camera.
    ///     Lets us offset the camera when in shift lock.
    /// </summary>
    Vector3 OffsetPosition { get; }
}

public partial class PlayerCamera : Node3D, IPlayerCamera
{
    public Vector3 SpringArmTargetPosition { get; }
    public Vector3 CameraLocalPosition { get; }
    public Vector3 GimbalRotationHorizontal { get; }
    public Vector3 GimbalRotationVertical { get; }
    public Basis CameraBasis { get; }
    public Vector3 OffsetPosition { get; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}