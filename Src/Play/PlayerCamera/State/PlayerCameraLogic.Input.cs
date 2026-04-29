using Godot;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
    public static class Input
    {
        public readonly record struct PhysicsTick(double Delta);
        public readonly record struct MouseInputOccurred(InputEventMouseMotion Motion);
        public readonly record struct ZoomedIn(float ZoomStrength);
        public readonly record struct ZoomedOut(float ZoomStrength);
        public readonly record struct FirstPersonEntered;
        public readonly record struct FirstPersonExited;
        public readonly record struct ToggleShiftLock;
        public readonly record struct RightClickPressed;
        public readonly record struct RightClickReleased;
    }
}