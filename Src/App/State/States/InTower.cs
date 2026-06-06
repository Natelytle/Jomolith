using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;
using static Jomolith.App.State.AppLogic;

namespace Jomolith.App.State.States;

public partial record AppState
{
    [Meta]
    public partial record InTower : AppState, IGet<Inputs.ExitTower>
    {
        public InTower()
        {
            this.OnEnter(() =>
            {
                Get<IAppRepo>().OnEnterTower();
                Output(new Outputs.EnterTower());
            });
            this.OnExit(() => Output(new Outputs.UnloadCurrentTower()));

            // TODO: Fix
            // OnAttach(() => Get<IAppRepo>().TowerExited += OnTowerExited);
            // OnDetach(() => Get<IAppRepo>().TowerExited -= OnTowerExited);
        }

        public void OnTowerExited()
        {
            Input(new Inputs.ExitTower());
        }

        public Type On(in Inputs.ExitTower input)
        {
            return To<ExitingTower>();
        }
    }
}
