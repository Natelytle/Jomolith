using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Landed : RunningBase, IGet<Input.TimerUp>
        {
            private const double jump_cooldown = 0.05;

            public Landed()
            {
                this.OnEnter(() => SetTimer(jump_cooldown));
            }

            public override Transition On(in Input.Jump input)
            {
                return ToSelf();
            }

            public Transition On(in Input.TimerUp input)
            {
                return To<Running>();
            }
        }
    }
}
