using Godot;

namespace Jomolith.Play.Player.Camera.PerspectiveState;

public partial class PerspectiveLogic
{
    public static class Input
    {
        public readonly record struct ToFirstPerson;

        public readonly record struct ToThirdPerson;
    }
}
