using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Game.Domain;

namespace Jomolith.Game.State;

public partial class GameLogic
{
    public partial record GameState
    {
        [Meta]
        public partial record InTower : GameState, IGet<Input.ExitTower>
        {
            public InTower()
            {
                this.OnEnter(() =>
                {
                    Get<IGameRepo>().OnEnterTower();
                    Output(new Output.EnterTower());
                });
                this.OnExit(() => Output(new Output.UnloadCurrentTower()));

                OnAttach(() => Get<IGameRepo>().TowerExited += OnTowerExited);
                OnDetach(() => Get<IGameRepo>().TowerExited -= OnTowerExited);
            }

            public void OnTowerExited()
            {
                Input(new Input.ExitTower());
            }

            public Transition On(in Input.ExitTower input)
            {
                return To<ExitingTower>();
            }
        }
    }
}
