using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.App.State.AppLogic;

namespace Jomolith.App.State.States;

public partial record AppState
{
    [Meta]
    public partial record LoadingTower : AppState, IGet<Inputs.TowerLoaded>
    {
        public LoadingTower()
        {
            this.OnEnter(() =>
            {
                Output(new Outputs.StartLoadingTower());
            });
        }

        public Type On(in Inputs.TowerLoaded input)
        {
            return To<InTower>();
        }
    }
}
