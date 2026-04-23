using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Idle : RunningBase, IGet<Input.FacingLadder>, IGet<Input.IsMoving>, IGet<Input.OffFloor>
        {
            public Idle()
            {
                this.OnEnter(() => Output(new Output.Animations.Idle()));
            }
            
            public Transition On(in Input.FacingLadder input)
            {
                return To<Climbing>();
            }

            public Transition On(in Input.IsMoving input)
            {
                return To<Running>();
            }

            public Transition On(in Input.OffFloor input)
            {
                return To<Coyote>();
            }
        }
    }
}