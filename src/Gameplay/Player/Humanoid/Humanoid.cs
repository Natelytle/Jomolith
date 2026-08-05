using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Gameplay.Domain;
using Jomolith.Gameplay.Player.Domain;
using Jomolith.Gameplay.Player.Humanoid.State;
using Jomolith.Gameplay.Player.Humanoid.State.States;

namespace Jomolith.Gameplay.Player.Humanoid;

public interface IHumanoid : IRigidBody3D, IProvide<IPlayerLogic>
{
    Vector3 GlobalRootPosition { get; }

    void PhysicsTick(double delta);

    Vector2 GetGlobalInputVector(Basis cameraBasis);

    Vector3 GetNoclipInputVector(Basis cameraBasis);

    FloorData GetFloorData(bool wasOnFloor);

    bool IsClimbing();
}

[Meta(typeof(IAutoNode))]
public partial class Humanoid : RigidBody3D, IHumanoid
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] private IPlayerRepo playerRepo => this.DependOn<IPlayerRepo>();
    [Dependency] private IGameplayRepo gameplayRepo => this.DependOn<IGameplayRepo>();

    #endregion

    #region Provisions

    IPlayerLogic IProvide<IPlayerLogic>.Value() => playerLogic;

    #endregion

    #region State

    private IPlayerLogic playerLogic { get; set; } = null!;

    private PlayerLogic.PlayerData playerData { get; set; } = null!;

    private PlayerLogic.PlayerSettings settings { get; set; } = null!;

    #endregion

    #region Nodes

    [Node("%PlayerModel")] private IPlayerModel playerModel { get; set; } = null!;

    [Node("%RootPart")] private INode3D rootPart { get; set; } = null!;

    [Node("%LegRaycast")] private IRayCast3D legRaycast { get; set; } = null!;

    [Node("%ClimbRaycast")] private IRayCast3D climbRaycast { get; set; } = null!;

    [Node("%HeadRaycast")] private IRayCast3D headRaycast { get; set; } = null!;

    [Node("%HeadCollisionShape")] private ICollisionShape3D headCollisionShape { get; set; } = null!;

    [Node("%TorsoCollisionShape")] private ICollisionShape3D torsoCollisionShape { get; set; } = null!;

    #endregion

    #region Computed

    public Vector3 GlobalRootPosition => rootPart.GlobalPosition;
    public Vector3 LocalRootPosition => rootPart.Position;

    #endregion

    public void Setup()
    {
        playerData = new PlayerLogic.PlayerData();
        settings = new PlayerLogic.PlayerSettings(1.0f, 16.0f, 50.0f);
        playerLogic = new PlayerLogic();

        playerLogic.Set(this as IHumanoid);
        playerLogic.Set(settings);
        playerLogic.Set(playerRepo);
        playerLogic.Set(playerData);
        playerLogic.Set(gameplayRepo);

        playerModel.HeadMoved += updateHeadTransform;
        playerModel.TorsoMoved += updateTorsoTransform;
    }

    // Called when the node enters the scene tree for the first time.
    public void OnResolved()
    {
        playerLogic.Bind()
            .OnOutput((in PlayerLogic.Outputs.ApplyForce output) =>
            {
                ApplyCentralForce(output.Force);
                ApplyTorque(output.Torque);
            })
            .OnOutput((in PlayerLogic.Outputs.SetRotation output) =>
            {
                Rotation = output.Rotation;
            })
            .OnOutput((in PlayerLogic.Outputs.SetFrozen output) =>
            {
                Freeze = output.Frozen;
            })
            .OnOutput((in PlayerLogic.Outputs.SetTransform output) =>
            {
                GlobalTransform = output.NewTransform;
            })
            .OnOutput((in PlayerLogic.Outputs.SetFriction output) =>
            {
                PhysicsMaterialOverride.Friction = output.Friction;
            });

        this.Provide();

        playerLogic.Start<PlayerState.Disabled>();
    }

    public void PhysicsTick(double delta)
    {
        playerLogic.Input(new PlayerLogic.Inputs.PhysicsTick(delta));

        if (Input.IsActionPressed("jump"))
        {
            playerLogic.Input(new PlayerLogic.Inputs.Jump());
        }

        if (Input.IsActionJustPressed("toggle_noclip"))
        {
            playerLogic.Input(new PlayerLogic.Inputs.ToggleNoclip());
        }
    }

    public Vector2 GetGlobalInputVector(Basis cameraBasis)
    {
        Vector2 inputDir = Input.GetVector("move_left", "move_right", "move_forward", "move_back");
        Vector3 rotated =
            (GlobalBasis.Rotated(Vector3.Up, cameraBasis.GetEuler().Y - Rotation.Y) *
             new Vector3(inputDir.X, 0, inputDir.Y)) with
            {
                Y = 0
            };

        return new Vector2(rotated.X, rotated.Z);
    }

    public Vector3 GetNoclipInputVector(Basis cameraBasis)
    {
        float xAxis = Input.GetAxis("move_left", "move_right");
        float yAxis = Input.GetAxis("move_down", "move_up");
        float zAxis = Input.GetAxis("move_forward", "move_back");

        Vector3 vector = cameraBasis * new Vector3(xAxis, yAxis, zAxis).Normalized();

        return vector;
    }

    public FloorData GetFloorData(bool wasOnFloor)
    {
        float[] xPositions = [0, 0.8f, -0.8f];
        float[] zPositions = [0, -0.4f, 0.4f];

        const float y_offset = -0.9f;
        float yPosition = LocalRootPosition.Y + y_offset;

        // Get the raycast length depending on if we had a floor last frame.
        float length = wasOnFloor ? 1.5f : 1.1f;
        length += Math.Abs(LinearVelocity.Y) > 100 ? Math.Abs(LinearVelocity.Y) / 100.0f : 0;
        length = length * 2 + 1;

        legRaycast.TargetPosition = new Vector3(0, -length, 0);

        Vector3? floorNormal = null;
        Vector3? floorLocation = null;
        Vector3 floorVelocity = Vector3.Zero;

        Vector3 floorHitLocationSum = Vector3.Zero;
        int count = 0;

        // Check the center, then the sides.
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                // We skip the center raycast on the sides...
                if (i > 0 && j == 0)
                    continue;

                legRaycast.Position = new Vector3(xPositions[i], yPosition, zPositions[j]);
                legRaycast.ForceRaycastUpdate();

                if (legRaycast.IsColliding())
                {
                    Vector3 hitNormal = legRaycast.GetCollisionNormal();

                    // Ignore walls
                    if (hitNormal.AngleTo(Vector3.Up) > float.DegreesToRadians(89.9f))
                        continue;

                    floorHitLocationSum += legRaycast.GetCollisionPoint();
                    count++;

                    floorNormal ??= legRaycast.GetCollisionNormal();
                    floorLocation ??= legRaycast.GetCollisionPoint();
                }
            }

            if (count != 0)
                break;
        }

        const float z_position_secondary = 0.8f;

        // We have 2 more checks, just do em manually
        if (floorHitLocationSum.LengthSquared() > 0)
        {
            for (int i = -1; i < 2; i += 2)
            {
                legRaycast.Position = new Vector3(0, yPosition, i * z_position_secondary);
                legRaycast.ForceRaycastUpdate();

                if (legRaycast.IsColliding())
                {
                    floorHitLocationSum += legRaycast.GetCollisionPoint();
                    count++;
                }
            }
        }

        if (count > 0)
            floorLocation = floorHitLocationSum / count;

        return new FloorData
        {
            FloorNormal = floorNormal,
            FloorPosition = floorLocation,
            FloorVelocity = floorVelocity
        };
    }

    public bool IsClimbing()
    {
        const float start_offset = 2.7f;
        const float y_position_increments = 1 / 7.0f;
        const float y_position_initial = -start_offset + y_position_increments;

        const float z_search_length_truss = 1.05f;
        const float z_search_length_ladder = 0.7f;

        Vector3 heightOffset = Vector3.Zero;

        climbRaycast.Position = LocalRootPosition;
        climbRaycast.TargetPosition = new Vector3(0, -start_offset, 0);
        climbRaycast.ForceRaycastUpdate();

        if (climbRaycast.IsColliding())
        {
            heightOffset.Y = start_offset - (climbRaycast.GlobalPosition.Y - climbRaycast.GetCollisionPoint().Y);
        }

        // TODO: Searching for trusses

        // Searching for ladders
        bool hitUnderCyanRaycast = false;
        bool airOverFirstHit = false;
        int distanceOfAirFromFirstHit = 0;
        bool redRaysHit = false;
        bool secondHitExists = false;

        climbRaycast.TargetPosition = new Vector3(0, 0, -z_search_length_ladder);

        for (int i = 0; i < 27; i++)
        {
            climbRaycast.Position = LocalRootPosition + heightOffset + new Vector3(0, y_position_initial + i * y_position_increments, 0);
            climbRaycast.ForceRaycastUpdate();

            if (i < 3 && climbRaycast.IsColliding())
            {
                redRaysHit = true;
            }

            if (i < 17 && climbRaycast.IsColliding())
            {
                hitUnderCyanRaycast = true;
            }

            if (hitUnderCyanRaycast && climbRaycast.IsColliding())
            {
                distanceOfAirFromFirstHit++;
            }

            if (hitUnderCyanRaycast && !climbRaycast.IsColliding() && distanceOfAirFromFirstHit < 17)
            {
                airOverFirstHit = true;
            }

            if (redRaysHit && i < 26 && airOverFirstHit && climbRaycast.IsColliding())
            {
                secondHitExists = true;
            }
        }

        return hitUnderCyanRaycast && airOverFirstHit && (!redRaysHit || secondHitExists);
    }

    private void updateHeadTransform(Transform3D newTransform)
    {
        headCollisionShape.GlobalTransform = newTransform;
    }

    private void updateTorsoTransform(Transform3D newTransform)
    {
        torsoCollisionShape.GlobalTransform = newTransform;
    }
}
