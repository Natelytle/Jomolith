using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;

namespace Jomolith.App.State;

public partial class AppLogic
{
    public partial record AppState
    {
        [Meta]
        public partial record InTower : AppState, IGet<Input.ExitTower>
        {
            public InTower()
            {
                this.OnEnter(() =>
                {
                    Get<IAppRepo>().OnEnterTower();
                    Output(new Output.EnterTower());
                });
                this.OnExit(() => Output(new Output.UnloadCurrentTower()));

                OnAttach(() => Get<IAppRepo>().TowerExited += OnTowerExited);
                OnDetach(() => Get<IAppRepo>().TowerExited -= OnTowerExited);
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
