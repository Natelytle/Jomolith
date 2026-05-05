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
        public Transition On(in Input.PhysicsTick input)
        {
            IHumanoid player = Get<IHumanoid>();
            IPlayerRepo playerRepo = Get<IPlayerRepo>();
            PlayerData playerData = Get<PlayerData>();

            Input(new Input.ComputeForces(input.Delta));
            Input(new Input.PhysicsTickAlive(input.Delta));

            Basis cameraBasis = playerRepo.CameraBasis.Value;
            Vector2 desiredMoveDirection = player.GetGlobalInputVector(cameraBasis);

            Input(new Input.DesiredMovementVector(desiredMoveDirection));

            // Raycasts
            FloorData floorData = player.GetFloorData(playerData.WasOnFloor);

            if (floorData.FloorFound)
            {
                if (!playerData.WasOnFloor)
                    Input(new Input.HitFloor());

                Input(new Input.OnFloor(floorData));
            }
            else if (playerData.WasOnFloor)
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
            if (playerData.Timer > 0 && playerData.Timer - input.Delta < 0)
                Input(new Input.TimerUp());

            playerData.Timer -= input.Delta;

            // Update player position
            playerRepo.SetPlayerGlobalPosition(player.GlobalRootPosition);
            playerRepo.SetPlayerBasis(player.GlobalBasis);

            return ToSelf();
        }
    }
}
