using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.State;

public partial class GameLogic
{
    [Meta]
    public abstract partial record GameState : StateLogic<GameState>;
}