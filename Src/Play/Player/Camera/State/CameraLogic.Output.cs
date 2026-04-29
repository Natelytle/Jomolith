using Godot;

namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
{
    public static class Output
    {
        public readonly record struct ShiftLockEntered(bool FirstPerson);
        public readonly record struct ShiftLockExited(bool FirstPerson);
        public readonly record struct FirstPersonEntered(bool ShiftLock);
        public readonly record struct FirstPersonExited(bool ShiftLock);

        public readonly record struct GlobalPositionChanged(Vector3 GlobalPosition);
        public readonly record struct OffsetChanged(Vector3 Position);
        public readonly record struct RotationChanged(float HorizontalRotation, float VerticalRotation);
        public readonly record struct SpringLengthChanged(float Length);
    }
}