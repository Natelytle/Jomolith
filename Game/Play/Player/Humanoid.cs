using System.Collections.Generic;
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Jomolith.Game.Play.Player.State;

namespace Jomolith.Game.Play.Player;

public interface IHumanoid : IRigidBody3D
{
	IPlayerLogic PlayerLogic { get; }
	
	/// <summary>
	///   Uses the engine to determine the input vector, relative to the global
	///   camera direction.
	/// </summary>
	/// <param name="cameraBasis">Camera's global transform basis.</param>
	Vector3 GetGlobalInputVector(Basis cameraBasis);
}

public partial class Humanoid : RigidBody3D, IHumanoid
{
	public IPlayerLogic PlayerLogic { get; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
	
	public Vector3 GetGlobalInputVector(Basis cameraBasis)
	{
		throw new System.NotImplementedException();
	}
}