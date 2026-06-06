using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.App.State.AppLogic;

namespace Jomolith.App.State.States;

public partial record AppState
{
    [Meta]
    public partial record InMainMenu : AppState, IGet<Inputs.PlayTower>
    {
        public Type On(in Inputs.PlayTower input)
        {
            return To<LoadingTower>();
        }
    }
}
