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
            private const double rootPartHeight = 3f;

            public Falling()
            {
                this.OnEnter(() => Output(new Output.Animations.Fall()));
            }

            public Transition On(in Input.OnFloor input)
            {
                IHumanoid player = Get<IHumanoid>();

                double floorDistance = (input.FloorData.FloorPosition.GetValueOrDefault() - player.GlobalRootPosition).Length();

                if (floorDistance < rootPartHeight && player.LinearVelocity.Y <= 0)
                    return To<Running>();

                return ToSelf();
            }
        }
    }
}
