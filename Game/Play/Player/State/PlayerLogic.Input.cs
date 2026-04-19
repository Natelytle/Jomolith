namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public static class Input
    {
        public readonly record struct PhysicsTick(double Delta);
        public readonly record struct Jump(double Delta);
        public readonly record struct FacingLadder;
        public readonly record struct AwayLadder;
        public readonly record struct OnFloor;
        public readonly record struct InFloor;
        public readonly record struct OffFloor;
        public readonly record struct IsMoving;
        public readonly record struct IsIdle;
    }
}