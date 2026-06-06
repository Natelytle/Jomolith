using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Humanoid.Domain;
using Jomolith.Play.Player.Humanoid.State;
using Jomolith.Play.Player.Humanoid.State.States;

namespace Jomolith.Play.Player.Humanoid;

public interface IHumanoid : IRigidBody3D, IProvide<IPlayerLogic>
{
    IPlayerLogic PlayerLogic { get; }

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

    [Dependency] public IPlayerRepo PlayerRepo => this.DependOn<IPlayerRepo>();

    #endregion

    #region Provisions

    IPlayerLogic IProvide<IPlayerLogic>.Value() => PlayerLogic;

    #endregion

    #region State

    public IPlayerLogic PlayerLogic { get; set; } = null!;

    public PlayerLogic.PlayerData PlayerData { get; set; } = null!;

    public PlayerLogic.PlayerSettings Settings { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public IPlayerModel PlayerModel { get; set; } = null!;

    [Node] public INode3D RootPart { get; set; } = null!;

    [Node] public IRayCast3D LegRaycast { get; set; } = null!;

    [Node] public IRayCast3D ClimbRaycast { get; set; } = null!;

    [Node] public IRayCast3D HeadRaycast { get; set; } = null!;

    [Node] public ICollisionShape3D HeadCollisionShape { get; set; } = null!;

    [Node] public ICollisionShape3D TorsoCollisionShape { get; set; } = null!;

    #endregion

    #region Computed

    public Vector3 GlobalRootPosition => RootPart.GlobalPosition;
    public Vector3 LocalRootPosition => RootPart.Position;

    #endregion

    public void Setup()
    {
        PlayerData = new PlayerLogic.PlayerData();
        Settings = new PlayerLogic.PlayerSettings(1.0f, 16.0f, 50.0f);
        PlayerLogic = new PlayerLogic();

        PlayerLogic.Set(this as IHumanoid);
        PlayerLogic.Set(Settings);
        PlayerLogic.Set(PlayerRepo);
        PlayerLogic.Set(PlayerData);

        PlayerModel.HeadMoved += updateHeadTransform;
        PlayerModel.TorsoMoved += updateTorsoTransform;
    }

    // Called when the node enters the scene tree for the first time.
    public void OnResolved()
    {
        using var binding = PlayerLogic.Bind();

        binding
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

        PlayerLogic.Start<PlayerState.Disabled>();
    }

    public void PhysicsTick(double delta)
    {
        PlayerLogic.Input(new PlayerLogic.Inputs.PhysicsTick(delta));

        if (Input.IsActionPressed("Jump"))
        {
            PlayerLogic.Input(new PlayerLogic.Inputs.Jump());
        }

        if (Input.IsActionJustPressed("ToggleNoclip"))
        {
            PlayerLogic.Input(new PlayerLogic.Inputs.ToggleNoclip());
        }
    }

    public Vector2 GetGlobalInputVector(Basis cameraBasis)
    {
        Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
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
        float xAxis = Input.GetAxis("MoveLeft", "MoveRight");
        float yAxis = Input.GetAxis("MoveDown", "MoveUp");
        float zAxis = Input.GetAxis("MoveForward", "MoveBackward");

        Vector3 vector = cameraBasis * new Vector3(xAxis, yAxis, zAxis).Normalized();

        return vector;
    }

    public FloorData GetFloorData(bool wasOnFloor)
    {
        float[] xPositions = [0, 0.8f, -0.8f];
        float[] zPositions = [0, -0.4f, 0.4f];

        const float yOffset = -0.9f;
        float yPosition = LocalRootPosition.Y + yOffset;

        // Get the raycast length depending on if we had a floor last frame.
        float length = wasOnFloor ? 1.5f : 1.1f;
        length += Math.Abs(LinearVelocity.Y) > 100 ? Math.Abs(LinearVelocity.Y) / 100.0f : 0;
        length = length * 2 + 1;

        LegRaycast.TargetPosition = new Vector3(0, -length, 0);

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

                LegRaycast.Position = new Vector3(xPositions[i], yPosition, zPositions[j]);
                LegRaycast.ForceRaycastUpdate();

                if (LegRaycast.IsColliding())
                {
                    Vector3 hitNormal = LegRaycast.GetCollisionNormal();

                    // Ignore walls
                    if (hitNormal.AngleTo(Vector3.Up) > float.DegreesToRadians(89.9f))
                        continue;

                    floorHitLocationSum += LegRaycast.GetCollisionPoint();
                    count++;

                    floorNormal ??= LegRaycast.GetCollisionNormal();
                    floorLocation ??= LegRaycast.GetCollisionPoint();
                }
            }

            if (count != 0)
                break;
        }

        const float zPositionSecondary = 0.8f;

        // We have 2 more checks, just do em manually
        if (floorHitLocationSum.LengthSquared() > 0)
        {
            for (int i = -1; i < 2; i += 2)
            {
                LegRaycast.Position = new Vector3(0, yPosition, i * zPositionSecondary);
                LegRaycast.ForceRaycastUpdate();

                if (LegRaycast.IsColliding())
                {
                    floorHitLocationSum += LegRaycast.GetCollisionPoint();
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
        const float yPositionInitial = -2.7f + 1 / 7.0f;
        const float yPositionIncrements = 1 / 7.0f;
        const float zSearchLengthTruss = 1.05f;
        const float zSearchLengthLadder = 0.7f;

        // TODO: Searching for trusses

        // Searching for ladders
        bool hitUnderCyanRaycast = false;
        bool airOverFirstHit = false;
        int distanceOfAirFromFirstHit = 0;
        bool redRaysHit = false;
        bool secondHitExists = false;

        ClimbRaycast.TargetPosition = new Vector3(0, 0, -zSearchLengthLadder);

        for (int i = 0; i < 27; i++)
        {
            ClimbRaycast.Position = LocalRootPosition + new Vector3(0, yPositionInitial + i * yPositionIncrements, 0);
            ClimbRaycast.ForceRaycastUpdate();

            if (i < 3 && ClimbRaycast.IsColliding())
            {
                redRaysHit = true;
            }

            if (i < 17 && ClimbRaycast.IsColliding())
            {
                hitUnderCyanRaycast = true;
            }

            if (hitUnderCyanRaycast && ClimbRaycast.IsColliding())
            {
                distanceOfAirFromFirstHit++;
            }

            if (hitUnderCyanRaycast && !ClimbRaycast.IsColliding() && distanceOfAirFromFirstHit < 17)
            {
                airOverFirstHit = true;
            }

            if (redRaysHit && i < 26 && airOverFirstHit && ClimbRaycast.IsColliding())
            {
                secondHitExists = true;
            }
        }

        return hitUnderCyanRaycast && airOverFirstHit && (!redRaysHit || secondHitExists);
    }

    private void updateHeadTransform(Transform3D newTransform)
    {
        HeadCollisionShape.GlobalTransform = newTransform;
    }

    private void updateTorsoTransform(Transform3D newTransform)
    {
        TorsoCollisionShape.GlobalTransform = newTransform;
    }
}
