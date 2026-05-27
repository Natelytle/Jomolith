using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Play.Player.Humanoid.Domain;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Falling : FallingBase, IGet<Input.OnFloor>
        {
            private const double root_part_height = 3f;

            public Falling()
            {
                this.OnEnter(() => Output(new Output.Animations.Fall()));
            }

            public Transition On(in Input.OnFloor input)
            {
                IHumanoid player = Get<IHumanoid>();

                double floorDistance = (input.FloorData.FloorPosition.GetValueOrDefault() - player.GlobalRootPosition).Length();

                if (floorDistance < root_part_height && player.LinearVelocity.Y <= 0)
                    return To<Running>();

                return ToSelf();
            }
        }
    }
}
