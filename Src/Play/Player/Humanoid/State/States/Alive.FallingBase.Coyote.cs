using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Play.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Play.Player.Humanoid.State.States;

public partial record PlayerState
{
    // TODO: Coyote state doesn't let you move (causes walkoffs and preserves velocity before gravity happens when walking up slopes)
    [Meta]
    public partial record Coyote : FallingBase, IGet<Inputs.Jump>, IGet<Inputs.TimerUp>
    {
        private const double coyote_time = 0.125;

        public Coyote()
        {
            this.OnEnter(() => setTimer(coyote_time));
        }

        public Type On(in Inputs.Jump input)
        {
            return To<Jumping>();
        }

        public Type On(in Inputs.TimerUp input)
        {
            return To<Falling>();
        }

        // Skip the landing state when we're back on the ground from Coyote
        public override Type On(in Inputs.HitFloor input)
        {
            return To<Running>();
        }
    }
}
