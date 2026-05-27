using System;
using Chickensoft.Introspection;
using Godot;

namespace Jomolith.Play.Player.Humanoid.State;

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

            public virtual Transition On(in Input.OnFloor input)
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
                    Vector3 groundForce = Vector3.Up * 110 * (desiredYVelocity.Value - player.LinearVelocity.Y) *
                                          player.Mass;

                    Output(new Output.ApplyForce(antiGravityForce + groundForce, Vector3.Zero));
                }

                return ToSelf();
            }

            protected override float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player, bool isRotationLocked)
            {
                // No torque in shift lock in the running state.
                float angleDifference = isRotationLocked ? 0 : playerData.PlayerHeading.SignedAngleTo(movementVector, Vector3.Up);
                float desiredAngVel = 8.0f * Math.Abs(angleDifference) * Math.Sign(angleDifference);

                return 100.0f * player.GetInertia().Y * (desiredAngVel - player.AngularVelocity.Y);
            }
        }
    }
}
