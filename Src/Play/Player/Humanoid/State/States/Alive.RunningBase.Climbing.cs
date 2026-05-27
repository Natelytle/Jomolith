
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
        public partial record Climbing : RunningBase, IGet<Input.OffFloor>, IGet<Input.AwayLadder>
        {
            private bool onGround;
            private const float move_speed_multiplier = 0.7f;

            public Climbing()
            {
                this.OnEnter(() =>
                {
                    IHumanoid player = Get<IHumanoid>();

                    Output(new Output.Animations.Climb());

                    player.GravityScale = 0;
                });

                this.OnExit(() =>
                {
                    IHumanoid player = Get<IHumanoid>();
                    PlayerSettings settings = Get<PlayerSettings>();

                    player.GravityScale = settings.GravityScale;
                });
            }

            public override Transition On(in Input.DesiredMovementVector input)
            {
                IHumanoid player = Get<IHumanoid>();
                PlayerSettings settings = Get<PlayerSettings>();
                PlayerData playerData = Get<PlayerData>();
                IPlayerRepo playerRepo = Get<IPlayerRepo>();

                Vector3 moveDirection = new Vector3(input.DesiredMovement.X, 0, input.DesiredMovement.Y) * 16.0f;

                Vector3 targetMovementVector = moveDirection.AngleTo(playerData.PlayerHeading) > float.DegreesToRadians(100f)
                                              ? Vector3.Down
                                              : Vector3.Up;

                // Scale by movespeed
                targetMovementVector *= settings.MoveSpeed * move_speed_multiplier * input.DesiredMovement.Length();

                Vector3 correctionVector = targetMovementVector - new Vector3(player.LinearVelocity.X, player.LinearVelocity.Y, player.LinearVelocity.Z);
                correctionVector = correctionVector.Normalized() * Math.Min(MaxForce, Gain * correctionVector.Length());
                Vector3 desiredForce = correctionVector * player.Mass;

                float desiredTorque = ComputeTorque(targetMovementVector, playerData, player, playerRepo.IsPlayerRotationLocked.Value);

                Output(new Output.ApplyForce(desiredForce, Vector3.Up * desiredTorque));

                return ToSelf();
            }

            public override Transition On(in Input.OnFloor input)
            {
                onGround = true;

                return ToSelf();
            }

            public Transition On(in Input.OffFloor input)
            {
                onGround = false;

                return ToSelf();
            }

            public override Transition On(in Input.Jump input)
            {
                Get<PlayerData>().WasClimbingBeforeJump = true;

                return To<Jumping>();
            }

            protected override float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player, bool isRotationLocked)
            {
                return -100.0f * player.GetInertia().Y * player.AngularVelocity.Y;
            }

            public Transition On(in Input.AwayLadder input)
            {
                return onGround ? To<Running>() : To<Falling>();
            }
        }
    }
}
