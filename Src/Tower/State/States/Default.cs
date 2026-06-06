using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Tower.State.TowerLogic;

namespace Jomolith.Tower.State.States;

public partial record TowerState
{
    [Meta]
    public partial record Default : TowerState,
    IGet<Inputs.Default>
    {
        public Default()
        {
            this.OnEnter(() =>
            {
                // EDIT ME
            });

            this.OnExit(() =>
            {
                // EDIT ME
            });
        }

        public Type On(in Inputs.Default input)
        {
            // EDIT ME
            return ToSelf();
        }
    }
}
