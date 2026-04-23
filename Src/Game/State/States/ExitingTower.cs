using Chickensoft.Introspection;

namespace Jomolith.Game.State;

public partial class GameLogic
{
    public partial record GameState
    {
        [Meta]
        public partial record ExitingTower : GameState, IGet<Input.MainMenuRequested>
        {
            public Transition On(in Input.MainMenuRequested input)
            {
                return To<InMainMenu>();
            }
        }
    }
}