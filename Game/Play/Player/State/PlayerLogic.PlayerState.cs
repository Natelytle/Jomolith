using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    [Meta]
    public abstract partial record PlayerState : StateLogic<PlayerState>;
}