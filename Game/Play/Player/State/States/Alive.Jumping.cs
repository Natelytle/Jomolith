using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Jumping : Alive, 
            IGet<Input.OffFloor>,
            IGet<Input.TimerUp>
        {
            private const double max_jump_time = 0.5;
            private const float jump_power_multiplier = 1.06f;

            // When in the jumping state, we can't really move and the character quits balancing.
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

                    Input(new Input.SetTimer(max_jump_time));
                    Output(new Output.Animations.Jump());
                });
            }

            public Transition On(in Input.OffFloor input)
            {
                return To<Falling>();
            }

            public override Transition On(in Input.ComputeForces input)
            {
                PlayerData playerData = Get<PlayerData>();
                IHumanoid player = Get<IHumanoid>();

                Vector3 jumpDirection = wasClimbing ? (Vector3.Up - playerData.PlayerHeading).Normalized() : Vector3.Up;

                float currentJumpVelocity = player.LinearVelocity.Dot(jumpDirection);
                float desiredJumpAcceleration = float.Round(1.0f / (float)input.Delta) * (desiredJumpSpeed - currentJumpVelocity);

                if (playerData.HittingCeiling || desiredJumpAcceleration <= 0)
                    return To<Falling>();
                
                float desiredJumpForce = desiredJumpAcceleration * player.Mass;
                Vector3 antiGravityForce = -player.GetGravity() * player.Mass;

                Output(new Output.ApplyForce(jumpDirection * desiredJumpForce + antiGravityForce, Vector3.Zero));

                return ToSelf();
            }

            public Transition On(in Input.TimerUp input)
            {
                return To<Falling>();
            }
        }
    }
}