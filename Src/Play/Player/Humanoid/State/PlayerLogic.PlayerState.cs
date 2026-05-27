using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Humanoid.Domain;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    [Meta]
    public abstract partial record PlayerState : StateLogic<PlayerState>, IGet<Input.PhysicsTick>
    {
        private double timer;

        public Transition On(in Input.PhysicsTick input)
        {
            IHumanoid player = Get<IHumanoid>();
            IPlayerRepo playerRepo = Get<IPlayerRepo>();
            PlayerData playerData = Get<PlayerData>();

            ComputeForces(input.Delta);
            ProcessPhysics(input.Delta);

            Basis cameraBasis = playerRepo.CameraBasis.Value;
            Vector2 desiredMoveDirection = player.GetGlobalInputVector(cameraBasis);

            Input(new Input.DesiredMovementVector(desiredMoveDirection));

            // Raycasts
            FloorData floorData = player.GetFloorData(playerData.WasOnFloor);

            if (floorData.FloorFound)
            {
                Input(new Input.HitFloor(player.LinearVelocity.Y));

                Input(new Input.OnFloor(floorData));
            }
            else
            {
                Input(new Input.OffFloor());
            }

            playerData.WasOnFloor = floorData.FloorFound;
            playerData.FloorNormal = floorData.FloorNormal;
            playerData.FloorPosition = floorData.FloorPosition;
            playerData.FloorVelocity = floorData.FloorVelocity;

            // Set player statistics
            playerData.PlayerHeading = new Plane(Vector3.Up).Project(-player.Basis.Z).Normalized();

            // Check & update our timer
            if (timer > 0 && timer - input.Delta <= 0)
                Input(new Input.TimerUp());

            timer -= input.Delta;

            // Update player position
            playerRepo.SetPlayerGlobalPosition(player.GlobalRootPosition);
            playerRepo.SetPlayerBasis(player.GlobalBasis);

            return ToSelf();
        }

        public virtual void ProcessPhysics(double delta) { }

        public virtual void ComputeForces(double delta) { }

        protected void SetTimer(double time) => timer = time;
    }
}
