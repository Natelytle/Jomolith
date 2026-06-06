using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.App.State.AppLogic;

namespace Jomolith.App.State.States;

public partial record AppState
{
    [Meta]
    public partial record ExitingTower : AppState, IGet<Inputs.MainMenuRequested>
    {
        public Type On(in Inputs.MainMenuRequested input)
        {
            return To<InMainMenu>();
        }
    }
}
