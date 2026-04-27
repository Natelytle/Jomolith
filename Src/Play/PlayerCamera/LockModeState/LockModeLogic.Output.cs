using Godot;

namespace Jomolith.Play.PlayerCamera.LockModeState;

public partial class LockModeLogic
{
    public static class Output
    {
        public readonly record struct ShiftLockEntered;
        public readonly record struct ShiftLockExited;
    }
}