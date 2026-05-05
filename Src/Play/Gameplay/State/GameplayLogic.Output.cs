using Godot;

namespace Jomolith.Play.Gameplay.State;

public partial class GameplayLogic
{
    public static class Output
    {
        public readonly record struct SetMouseCaptureMode(bool IsMouseCaptured);

        public readonly record struct SetPaused(bool IsPaused);
    }
}
