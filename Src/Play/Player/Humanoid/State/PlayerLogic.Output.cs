using Godot;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public static class Output
    {
        public static class Animations
        {
            public readonly record struct Idle;
            public readonly record struct Walk;
            public readonly record struct Jump;
            public readonly record struct Fall;
            public readonly record struct Climb;
            public readonly record struct Disabled;
            public readonly record struct Enabled;
        }

        public readonly record struct ApplyForce(Vector3 Force, Vector3 Torque);

        public readonly record struct SetRotation(Vector3 Rotation);

        public readonly record struct FloorVelocityChanged(Vector2 Velocity);

        public readonly record struct VerticalVelocityChanged(Vector2 Velocity);

        public readonly record struct SetFrozen(bool Frozen);
    }
}