using Godot;
using Jomolith.Play.Player.Humanoid.Domain;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public static class Input
    {
        public readonly record struct PhysicsTick(double Delta);
        public readonly record struct PhysicsTickAlive(double Delta);
        public readonly record struct ComputeForces(double Delta);
        public readonly record struct DesiredMovementVector(Vector2 DesiredMovement);
        public readonly record struct Jump;
        public readonly record struct FacingLadder;
        public readonly record struct AwayLadder;
        public readonly record struct HitFloor;
        public readonly record struct OnFloor(FloorData FloorData);
        public readonly record struct OffFloor;
        public readonly record struct IsMoving;
        public readonly record struct IsIdle;
        public readonly record struct SetTimer(double Time);
        public readonly record struct TimerUp;
        public readonly record struct Enable;
    }
}