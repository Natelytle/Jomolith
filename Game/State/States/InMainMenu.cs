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
            public InMainMenu()
            {
                this.OnEnter(() =>
                    {
                        Get<Data>().ShouldLoadExistingGame = false;

                        Get<IGameRepo>().OnMainMenuEntered();

                        Output(new Output.ShowMainMenu());
                    }
                );
            }

            public Transition On(in Input.PlayTower input)
            {
                return To<LoadingTower>();
            }
        }
    }
}