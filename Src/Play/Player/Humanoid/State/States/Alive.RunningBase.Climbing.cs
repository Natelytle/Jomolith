using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Climbing : RunningBase
        {
            public Climbing()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.Animations.Climb());
                });
            }

            public override Transition On(in Input.DesiredMovementVector input)
            {
                return base.On(in input);
            }

            public override Transition On(in Input.Jump input)
            {
                Get<PlayerData>().WasClimbingBeforeJump = true;

                return To<Jumping>();
            }
        }
    }
}