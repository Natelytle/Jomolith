using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public abstract partial record RunningBase : Alive, IGet<Input.Jump>, IGet<Input.OnFloor>
        {
            protected override float MaxForce => 741.6f;
            protected override float Gain => 150.0f;
            protected override float BalanceKp => 7000.0f;
            protected override float BalanceKd => 100.0f;

            public virtual Transition On(in Input.Jump input)
            {
                return To<Jumping>();
            }

            public Transition On(in Input.OnFloor input)
            {
                IHumanoid player = Get<IHumanoid>();
                PlayerData playerData = Get<PlayerData>();

                // The root position is 3 studs above the ground
                const float rootHeight = 3f;

                float? desiredAltitude = playerData.FloorPosition?.Y + rootHeight;
                float? desiredYVelocity = 27 * (desiredAltitude - player.GlobalRootPosition.Y);

                if (desiredYVelocity != null && desiredYVelocity > 0)
                {
                    Vector3 antiGravityForce = -player.GetGravity() * player.Mass;
                    Vector3 groundForce = Vector3.Up * 110 * (desiredYVelocity.Value - player.LinearVelocity.Y) * player.Mass;

                    Output(new Output.ApplyForce(antiGravityForce + groundForce, Vector3.Zero));
                }

                return ToSelf();
            }
        }
    }
}