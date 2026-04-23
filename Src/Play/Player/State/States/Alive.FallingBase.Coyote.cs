using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        // TODO: Coyote state doesn't let you move (causes walkoffs and preserves velocity before gravity happens when walking up slopes)
        [Meta]
        public partial record Coyote : FallingBase, IGet<Input.Jump>, IGet<Input.TimerUp>
        {
            private const double coyote_time = 0.125;

            public Coyote() 
            {
                this.OnEnter(() => Input(new Input.SetTimer(coyote_time)));
            }

            public Transition On(in Input.Jump input)
            {
                return To<Jumping>();
            }

            public Transition On(in Input.TimerUp input)
            {
                return To<Falling>();
            }
            
            // Skip the landing state when we're back on the ground from Coyote
            public override Transition On(in Input.HitFloor input)
            {
                return To<Running>();
            }
        }
    }
}