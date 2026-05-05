using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Gameplay.State;

public partial class GameplayLogic
{
    [Meta]
    public partial record GameplayState : StateLogic<GameplayState>
    {
    }
}
