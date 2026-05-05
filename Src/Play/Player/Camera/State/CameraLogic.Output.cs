using Godot;

namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
{
    public static class Output
    {
        public readonly record struct SetRightClickPressed(bool Value);
        public readonly record struct SetCameraLocked(bool Value);
        public readonly record struct SetPlayerLocked(bool Value);

        public readonly record struct GlobalPositionChanged(Vector3 GlobalPosition);
        public readonly record struct OffsetChanged(Vector3 Position);
        public readonly record struct RotationChanged(float HorizontalRotation, float VerticalRotation);
        public readonly record struct SpringLengthChanged(float Length);
    }
}
