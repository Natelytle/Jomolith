using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Gameplay.Player;

public interface IPlayer : INode3D;

public partial class Player : Node3D, IPlayer
{

}
