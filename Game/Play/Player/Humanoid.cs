using System;
using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Game.Play.Player.State;
using Jomolith.Game.Play.Player.Utils;
using Jomolith.Game.Play.Tower.Domain;

namespace Jomolith.Game.Play.Player;

public interface IHumanoid : IRigidBody3D, IProvide<IPlayerLogic>
{
	IPlayerLogic PlayerLogic { get; }
	
	Vector3 GlobalRootPosition { get; }
	
	Vector3 LocalRootPosition { get; }

	Vector2 GetGlobalInputVector(Basis cameraBasis);

	FloorData GetFloorData(bool wasOnFloor);
}

[Meta(typeof(IAutoNode))]
public partial class Humanoid : RigidBody3D, IHumanoid
{
	public override void _Notification(int what) => this.Notify(what);

	#region Dependencies

	[Dependency]
	public ITowerRepo TowerRepo => this.DependOn<ITowerRepo>();

	[Dependency]
	public IGameRepo GameRepo => this.DependOn<IGameRepo>();

	#endregion

	#region Provisions

	IPlayerLogic IProvide<IPlayerLogic>.Value() => PlayerLogic;

	#endregion

	#region State

	public IPlayerLogic PlayerLogic { get; set; } = null!;

	public Vector3 GlobalRootPosition => RootPart.GlobalPosition;
	public Vector3 LocalRootPosition => RootPart.Position;

	public PlayerLogic.PlayerData PlayerData { get; set; } = null!;

	public PlayerLogic.PlayerSettings Settings { get; set; } = null!;
	
	public PlayerLogic.IBinding PlayerBinding { get; set; } = null!;

	#endregion

	#region Nodes

	[Node] public IPlayerModel PlayerModel { get; set; } = null!;

	[Node] public INode3D RootPart { get; set; } = null!;

	[Node] public IRayCast3D LegRaycast { get; set; } = null!;
	
	[Node] public IRayCast3D ClimbRaycast { get; set; } = null!;
	
	[Node] public IRayCast3D HeadRaycast { get; set; } = null!;

	#endregion

	public void Setup()
	{
		PlayerData = new PlayerLogic.PlayerData();
		Settings = new PlayerLogic.PlayerSettings(1.0f, 16.0f, 50.0f);
		PlayerLogic = new PlayerLogic();
		
		PlayerLogic.Set(this as IHumanoid);
		PlayerLogic.Set(Settings);
		PlayerLogic.Set(TowerRepo);
		PlayerLogic.Set(GameRepo);
		PlayerLogic.Set(PlayerData);
	}

	// Called when the node enters the scene tree for the first time.
	public void OnResolved()
	{
		PlayerBinding = PlayerLogic.Bind();

		PlayerBinding.Handle((in PlayerLogic.Output.ApplyForce output) =>
		{
			ApplyCentralForce(output.Force);
			ApplyTorque(output.Torque);
		}).Handle((in PlayerLogic.Output.SetRotation output) =>
		{
			Rotation = output.Rotation;
		}).Handle((in PlayerLogic.Output.SetFrozen output) =>
		{
			Freeze = output.Frozen;
		});
		
		this.Provide();
		
		PlayerLogic.Start();
	}

	public void OnReady() => SetPhysicsProcess(true);

	public void OnPhysicsProcess(double delta)
	{
		PlayerLogic.Input(new PlayerLogic.Input.PhysicsTick(delta));

		if (Input.IsActionPressed("Jump"))
		{
			PlayerLogic.Input(new PlayerLogic.Input.Jump());
		}
	}
	
	public Vector2 GetGlobalInputVector(Basis cameraBasis)
	{
		Vector2 inputDir = Input.GetVector("MoveLeft", "MoveRight", "MoveForward", "MoveBackward");
		Vector3 rotated = (cameraBasis * new Vector3(inputDir.X, 0, inputDir.Y)) with { Y = 0 };

		return new Vector2(rotated.X, rotated.Z);
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
}