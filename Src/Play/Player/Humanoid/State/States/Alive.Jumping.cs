using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Jumping : Alive,
            IGet<Input.OffFloor>,
            IGet<Input.TimerUp>,
            IGet<Input.JumpFinished>
        {
            private const double max_jump_time = 0.5;
            private const float jump_power_multiplier = 1.06f;

            private const float jump_speed_epsilon = 1e-3f;

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

                    SetTimer(max_jump_time);
                    Output(new Output.Animations.Jump());
                });
            }

            public Transition On(in Input.OffFloor input)
            {
                return To<Falling>();
            }

            public override void ComputeForces(double delta)
            {
                PlayerData playerData = Get<PlayerData>();
                IHumanoid player = Get<IHumanoid>();

                Vector3 jumpDirection = wasClimbing ? (Vector3.Up - playerData.PlayerHeading).Normalized() : Vector3.Up;

                float currentJumpVelocity = player.LinearVelocity.Dot(jumpDirection);
                float desiredJumpAcceleration =
                    float.Round(1.0f / (float)delta) * (desiredJumpSpeed - currentJumpVelocity);

                if (playerData.HittingCeiling || desiredJumpSpeed - currentJumpVelocity <= jump_speed_epsilon)
                {
                    Input(new Input.JumpFinished());

                    return;
                }

                float desiredJumpForce = desiredJumpAcceleration * player.Mass;
                Vector3 antiGravityForce = -player.GetGravity() * player.Mass;

                Output(new Output.ApplyForce(jumpDirection * desiredJumpForce + antiGravityForce, Vector3.Zero));
            }

            // No input in the jumping state
            public override Transition On(in Input.DesiredMovementVector input) => ToSelf();

            // No torque in the jumping state.
            protected override float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player, bool isRotationLocked) => 0;

            public Transition On(in Input.TimerUp input) => To<Falling>();

            public Transition On(in Input.JumpFinished input) => To<Falling>();
        }
    }
}
