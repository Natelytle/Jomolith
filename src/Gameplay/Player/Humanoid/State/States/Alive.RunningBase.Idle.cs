using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Idle : RunningBase, IGet<Inputs.IsMoving>, IGet<Inputs.OffFloor>
    {
        public Idle()
        {
            this.OnEnter(() => Output(new Outputs.Animations.Idle()));
        }

        public Type On(in Inputs.IsMoving input)
        {
            return To<Running>();
        }

        public Type On(in Inputs.OffFloor input)
        {
            return To<Coyote>();
        }
    }
}
