using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Disabled : PlayerState, IGet<Inputs.Enable>
    {
        public Disabled()
        {
            this.OnEnter(() =>
            {
                Output(new Outputs.SetFrozen(true));
                Output(new Outputs.Animations.Disabled());
            });
        }

        public Type On(in Inputs.Enable input) => To<Idle>();
    }

    public void OnGameplayStarted() => Input(new Inputs.Enable());
}
