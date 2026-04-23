using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Game.Domain;

namespace Jomolith.Game.State;

public partial class GameLogic
{
    public partial record GameState
    {
        [Meta]
        public partial record InMainMenu : GameState, IGet<Input.PlayTower>
        {
            public Transition On(in Input.PlayTower input)
            {
                return To<LoadingTower>();
            }
        }
    }
}