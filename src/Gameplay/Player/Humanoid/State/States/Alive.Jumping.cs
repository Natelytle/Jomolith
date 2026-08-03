using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Jumping : Alive,
        IGet<Inputs.OffFloor>,
        IGet<Inputs.TimerUp>,
        IGet<Inputs.JumpFinished>
    {
        private const double max_jump_time = 0.5;
        private const float jump_power_multiplier = 1.06f;

        // Lower this value to increase the chance of a high jump on walls.
        private const float jump_accel_epsilon = 5e-4f;

        // Increase this value to decrease the chance of a high jump without touching a wall.
        private const float jump_accel_correction = 1e-3f;

        protected override float MaxForce => 0;
        protected override float Gain => 0;
        protected override float BalanceKp => 0;
        protected override float BalanceKd => 0;

        private bool wasClimbing;
        private float desiredJumpSpeed;

        public Jumping()
        {
            this.OnEnter(() =>
            {
                var playerData = Get<PlayerData>();
                wasClimbing = playerData.WasClimbingBeforeJump;
                playerData.WasClimbingBeforeJump = false;

                var playerSettings = Get<PlayerSettings>();
                desiredJumpSpeed = playerSettings.JumpPower * jump_power_multiplier;

                setTimer(max_jump_time);
                Output(new Outputs.Animations.Jump());
            });
        }

        public Type On(in Inputs.OffFloor input)
        {
            return To<Falling>();
        }

        protected override void ComputeForces(double delta)
        {
            PlayerData playerData = Get<PlayerData>();
            IHumanoid player = Get<IHumanoid>();

            Vector3 jumpDirection = wasClimbing ? (Vector3.Up - playerData.PlayerHeading).Normalized() : Vector3.Up;

            float currentJumpVelocity = player.LinearVelocity.Dot(jumpDirection);
            float desiredJumpAcceleration =
                float.Round(1.0f / (float)delta) * (desiredJumpSpeed - currentJumpVelocity);

            if (playerData.HittingCeiling || desiredJumpAcceleration <= jump_accel_epsilon)
            {
                Input(new Inputs.JumpFinished());

                return;
            }

            // If we aren't touching a wall, we give the character a little boost. Hacky but works for high jumps
            if (player.GetContactCount() == 0)
                desiredJumpAcceleration += jump_accel_correction;

            float desiredJumpForce = desiredJumpAcceleration * player.Mass;
            Vector3 antiGravityForce = -player.GetGravity() * player.Mass;

            Output(new Outputs.ApplyForce(jumpDirection * desiredJumpForce + antiGravityForce, Vector3.Zero));
        }

        // No input in the jumping state
        public override Type On(in Inputs.DesiredMovementVector input) => ToSelf();

        // No torque in the jumping state.
        protected override float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player, bool isRotationLocked) => 0;

        public Type On(in Inputs.TimerUp input) => To<Falling>();

        public Type On(in Inputs.JumpFinished input) => To<Falling>();
    }
}
