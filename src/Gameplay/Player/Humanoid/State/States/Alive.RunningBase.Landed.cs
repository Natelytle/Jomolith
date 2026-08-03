using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Landed : RunningBase, IGet<Inputs.TimerUp>
    {
        private const double jump_cooldown = 0.05;

        public Landed()
        {
            this.OnEnter(() => setTimer(jump_cooldown));
        }

        public override Type On(in Inputs.Jump input)
        {
            return ToSelf();
        }

        public Type On(in Inputs.TimerUp input)
        {
            return To<Running>();
        }
    }
}
