using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State.States;

[Meta, StateDiagram]
public abstract partial record MenuState : LogicBlockState, IGet<MenuLogic.Inputs.Global.BackButtonPressed>
{
    public Type On(in MenuLogic.Inputs.Global.BackButtonPressed input)
    {
        // Remove the current state from the stack
        Pop();
        var previousState = Pop();

        return previousState ?? ToSelf();
    }
}
