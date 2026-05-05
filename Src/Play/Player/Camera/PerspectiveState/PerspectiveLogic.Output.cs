using Godot;

namespace Jomolith.Play.Player.Camera.PerspectiveState;

public partial class PerspectiveLogic
{
    public static class Output
    {
        public readonly record struct FirstPersonEntered;

        public readonly record struct FirstPersonExited;
    }
}
