using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.State;

public partial class GameLogic
{
    public partial record GameState
    {
        [Meta]
        public partial record LoadingTower : GameState, IGet<Input.TowerLoaded>
        {
            public LoadingTower()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.StartLoadingTower());
                });
            }

            public Transition On(in Input.TowerLoaded input)
            {
                return To<InTower>();
            }
        }
    }
}
