using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Running : RunningBase, IGet<Input.FacingLadder>, IGet<Input.IsIdle>, IGet<Input.OffFloor>
        {
            public Running()
            {
                this.OnEnter(() => Output(new Output.Animations.Walk()));
            }

            public Transition On(in Input.FacingLadder input)
            {
                return To<Climbing>();
            }

            public Transition On(in Input.IsIdle input)
            {
                return To<Idle>();
            }

            public Transition On(in Input.OffFloor input)
            {
                return To<Coyote>();
            }
        }
    }
}
