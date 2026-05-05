using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Player.Domain;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public abstract partial record Alive : PlayerState,
            IGet<Input.PhysicsTickAlive>,
            IGet<Input.ComputeForces>,
            IGet<Input.DesiredMovementVector>,
            IGet<Input.SetTimer>
        {
            private const double idle_speed_threshold = 0.01;

            protected abstract float MaxForce { get; }
            protected abstract float Gain { get; }
            protected abstract float BalanceKp { get; }
            protected abstract float BalanceKd { get; }
            protected virtual float TurnAngleLimit => float.PositiveInfinity;

            public Alive()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.SetFrozen(false));
                    Output(new Output.Animations.Enabled());
                });
            }

            public Transition On(in Input.PhysicsTickAlive input)
            {
                var player = Get<IHumanoid>();
                var playerRepo = Get<IPlayerRepo>();

                // Calculate shift lock rotation
                if (playerRepo.IsPlayerRotationLocked.Value)
                {
                    float currentYaw = player.GlobalRotation.Y;
                    float desiredYaw = playerRepo.CameraBasis.Value.GetEuler().Y;
                    float angleDelta = Mathf.AngleDifference(currentYaw, desiredYaw);

                    Vector3 newRotation = player.Transform.Rotated(Vector3.Up, angleDelta).Basis.GetEuler();

                    Output(new Output.SetRotation(newRotation));
                }

                return ToSelf();
            }

            // Separate from the physics tick loop, since we want to change it in the jumping state
            public virtual Transition On(in Input.ComputeForces _)
            {
                IHumanoid player = Get<IHumanoid>();

                // Balancing
                Basis playerBasis = player.GlobalBasis;
                Vector3 tilt = Vector3.Up.Cross(playerBasis.Y);
                Vector3 playerAngularVelocity = player.AngularVelocity;

                Vector3 tiltLocal = tilt * playerBasis;
                Vector3 localAngularVelocity = playerAngularVelocity * playerBasis;

                Vector3 inertiaVector = player.GetInertia();

                Vector3 torqueLocal = -BalanceKp * (inertiaVector * tiltLocal) +
                                      -BalanceKd * (inertiaVector * localAngularVelocity);

                Vector3 appliedTorque = (playerBasis * torqueLocal) with { Y = 0 };

                Output(new Output.ApplyForce(Vector3.Zero, appliedTorque));

                return ToSelf();
            }

            // Separated from the physics tick loop, since we want to alter this behavior if we're climbing
            public virtual Transition On(in Input.DesiredMovementVector input)
            {
                IHumanoid player = Get<IHumanoid>();
                PlayerSettings settings = Get<PlayerSettings>();
                PlayerData playerData = Get<PlayerData>();

                Vector2 desiredMovementVector = input.DesiredMovement * settings.MoveSpeed;
                Vector3 targetMovementVector = new Vector3(desiredMovementVector.X, 0, desiredMovementVector.Y);
                Vector3 correctionVector = targetMovementVector -
                                           new Vector3(player.LinearVelocity.X, 0, player.LinearVelocity.Z);
                correctionVector = correctionVector.Normalized() * Math.Min(MaxForce, Gain * correctionVector.Length());
                Vector3 desiredForce = correctionVector * player.Mass;

                float angleToDesiredDirection =
                    playerData.PlayerHeading.SignedAngleTo(targetMovementVector, Vector3.Up);
                float desiredRotationalVelocity = 8.0f * Math.Min(Math.Abs(angleToDesiredDirection), TurnAngleLimit) *
                                                  Math.Sign(angleToDesiredDirection);
                float desiredTorque = 100.0f * player.GetInertia().Y *
                                      (desiredRotationalVelocity - player.AngularVelocity.Y);
                desiredTorque = Mathf.Clamp(desiredTorque, -1e5f, 1e5f);

                Output(new Output.ApplyForce(desiredForce, Vector3.Up * desiredTorque));

                Vector2 floorVelocity = new Vector2(player.LinearVelocity.X, player.LinearVelocity.Z);

                Output(new Output.FloorVelocityChanged(floorVelocity));

                if (floorVelocity.Length() > idle_speed_threshold)
                {
                    Input(new Input.IsMoving());
                }
                else
                {
                    Input(new Input.IsIdle());
                }

                return ToSelf();
            }

            public Transition On(in Input.SetTimer input)
            {
                PlayerData playerData = Get<PlayerData>();

                playerData.Timer = input.Time;

                return ToSelf();
            }
        }
    }
}
