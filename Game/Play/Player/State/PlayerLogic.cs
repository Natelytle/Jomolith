using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.Play.Player.State;

public interface IPlayerLogic : ILogicBlock<PlayerLogic.PlayerState>;

[Meta]
[LogicBlock(typeof(PlayerState), Diagram = true)]
public partial class PlayerLogic : LogicBlock<PlayerLogic.PlayerState>, IPlayerLogic
{
    public override Transition GetInitialState()
    {
        return To<PlayerState.Falling>();
    }
}