using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Play.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Play.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Falling : FallingBase, IGet<Inputs.OnFloor>
    {
        private const double root_part_height = 3f;

        public Falling()
        {
            this.OnEnter(() => Output(new Outputs.Animations.Fall()));
        }

        public Type On(in Inputs.OnFloor input)
        {
            IHumanoid player = Get<IHumanoid>();

            double floorDistance = (input.FloorData.FloorPosition.GetValueOrDefault() - player.GlobalRootPosition).Length();

            if (floorDistance < root_part_height && player.LinearVelocity.Y <= 0)
                return To<Running>();

            return ToSelf();
        }
    }
}
