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
            IGet<Input.DesiredMovementVector>,
            IGet<Input.ToggleNoclip>
        {
            private const double idle_speed_threshold = 0.01;

            protected abstract float MaxForce { get; }
            protected abstract float Gain { get; }
            protected abstract float BalanceKp { get; }
            protected abstract float BalanceKd { get; }

            public Alive()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.SetFrozen(false));
                    Output(new Output.Animations.Enabled());
                });
            }

            public virtual Transition On(in Input.DesiredMovementVector input)
            {
                IHumanoid player = Get<IHumanoid>();
                PlayerSettings settings = Get<PlayerSettings>();
                PlayerData playerData = Get<PlayerData>();
                IPlayerRepo playerRepo = Get<IPlayerRepo>();

                Vector2 desiredMovementVector = input.DesiredMovement * settings.MoveSpeed;
                Vector3 targetMovementVector = new Vector3(desiredMovementVector.X, 0, desiredMovementVector.Y);
                Vector3 correctionVector = targetMovementVector - new Vector3(player.LinearVelocity.X, 0, player.LinearVelocity.Z);
                correctionVector = correctionVector.Normalized() * Math.Min(MaxForce, Gain * correctionVector.Length());
                Vector3 desiredForce = correctionVector * player.Mass;

                float desiredTorque = ComputeTorque(targetMovementVector, playerData, player, playerRepo.IsPlayerRotationLocked.Value);

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

            protected abstract float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player,
                bool isRotationLocked);

            public Transition On(in Input.ToggleNoclip input)
            {
                return To<Noclip>();
            }

            public override void ProcessPhysics(double delta)
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
            }

            public override void ComputeForces(double delta)
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
            }
        }
    }
}
