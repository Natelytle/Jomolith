using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Running : RunningBase, IGet<Inputs.IsIdle>, IGet<Inputs.OffFloor>
    {
        public Running()
        {
            this.OnEnter(() => Output(new Outputs.Animations.Walk()));
        }

        public Type On(in Inputs.IsIdle input)
        {
            return To<Idle>();
        }

        public Type On(in Inputs.OffFloor input)
        {
            return To<Coyote>();
        }
    }
}
