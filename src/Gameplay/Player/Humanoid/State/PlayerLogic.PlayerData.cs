using Godot;

namespace Jomolith.Gameplay.Player.Humanoid.State;

public partial class PlayerLogic
{
    public record PlayerData
    {
        public bool WasOnFloor;
        public Vector3? FloorNormal;
        public Vector3? FloorPosition;
        public Vector3? FloorVelocity;

        public bool HittingCeiling;

        public bool WasClimbingBeforeJump;

        public Vector3 PlayerHeading;
        public Vector3 PlayerVelocity;
    }
}
