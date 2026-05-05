using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.State;

public interface IGameLogic : ILogicBlock<GameLogic.GameState>;

[Meta]
[LogicBlock(typeof(GameState), Diagram = true)]
public partial class GameLogic : LogicBlock<GameLogic.GameState>, IGameLogic
{
    public override Transition GetInitialState()
    {
        return To<GameState.InMainMenu>();
    }
}
