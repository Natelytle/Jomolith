using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Gameplay.Player.Domain;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

[Meta, StateDiagram]
public abstract partial record PlayerState : LogicBlockState,
    IGet<Inputs.PhysicsTick>
{
    private double timer;

    public Type On(in Inputs.PhysicsTick input)
    {
        IHumanoid player = Get<IHumanoid>();
        IPlayerRepo playerRepo = Get<IPlayerRepo>();
        PlayerData playerData = Get<PlayerData>();

        ComputeForces(input.Delta);
        ProcessPhysics(input.Delta);

        Basis cameraBasis = playerRepo.CameraBasis.Value;
        Vector2 desiredMoveDirection = player.GetGlobalInputVector(cameraBasis);

        Input(new Inputs.DesiredMovementVector(desiredMoveDirection));

        // Raycasts
        FloorData floorData = player.GetFloorData(playerData.WasOnFloor);

        if (floorData.FloorFound)
        {
            Input(new Inputs.HitFloor(player.LinearVelocity.Y));

            Input(new Inputs.OnFloor(floorData));
        }
        else
        {
            Input(new Inputs.OffFloor());
        }

        playerData.WasOnFloor = floorData.FloorFound;
        playerData.FloorNormal = floorData.FloorNormal;
        playerData.FloorPosition = floorData.FloorPosition;
        playerData.FloorVelocity = floorData.FloorVelocity;

        if (player.IsClimbing())
            Input(new Inputs.FacingLadder());
        else
            Input(new Inputs.AwayLadder());

        // Set player statistics
        playerData.PlayerHeading = new Plane(Vector3.Up).Project(-player.Basis.Z).Normalized();

        // Check & update our timer
        if (timer > 0 && timer - input.Delta <= 0)
            Input(new Inputs.TimerUp());

        timer -= input.Delta;

        // Update player position
        playerRepo.SetPlayerGlobalPosition(player.GlobalRootPosition);
        playerRepo.SetPlayerBasis(player.GlobalBasis);

        return ToSelf();
    }

    protected virtual void ProcessPhysics(double delta) { }

    protected virtual void ComputeForces(double delta) { }

    private void setTimer(double time) => timer = time;
}
