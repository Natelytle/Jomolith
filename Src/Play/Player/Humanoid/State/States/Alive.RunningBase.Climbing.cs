
using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Player.Domain;
using static Jomolith.Play.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Play.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Climbing : RunningBase, IGet<Inputs.OffFloor>, IGet<Inputs.AwayLadder>
    {
        private bool onGround;
        private const float move_speed_multiplier = 0.7f;

        protected override float MaxForce => onGround ? 741.6f : 143.0f;

        public Climbing()
        {
            this.OnEnter(() =>
            {
                IHumanoid player = Get<IHumanoid>();

                Output(new Outputs.Animations.Climb());

                player.GravityScale = 0;
            });

            this.OnExit(() =>
            {
                IHumanoid player = Get<IHumanoid>();
                PlayerSettings settings = Get<PlayerSettings>();

                player.GravityScale = settings.GravityScale;
            });
        }

        public override Type On(in Inputs.DesiredMovementVector input)
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

            // Horizontal correction gets clamped by MaxForce
            Vector3 correctionVector = -new Vector3(player.LinearVelocity.X, 0, player.LinearVelocity.Z);
            correctionVector = correctionVector.Normalized() * Math.Min(MaxForce, Gain * correctionVector.Length());

            // Vertical correction doesn't get clamped by MaxForce
            correctionVector += Vector3.Up * Gain * (targetMovementVector.Y - player.LinearVelocity.Y);

            Vector3 desiredForce = correctionVector * player.Mass;

            float desiredTorque = ComputeTorque(targetMovementVector, playerData, player, playerRepo.IsPlayerRotationLocked.Value);

            Output(new Outputs.ApplyForce(desiredForce, Vector3.Up * desiredTorque));

            float verticalVelocity = player.LinearVelocity.Y;

            Output(new Outputs.VerticalVelocityChanged(verticalVelocity));

            return ToSelf();
        }

        public override Type On(in Inputs.OnFloor input)
        {
            onGround = true;

            return ToSelf();
        }

        public Type On(in Inputs.OffFloor input)
        {
            onGround = false;

            return ToSelf();
        }

        public override Type On(in Inputs.Jump input)
        {
            Get<PlayerData>().WasClimbingBeforeJump = true;

            return To<Jumping>();
        }

        protected override float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player, bool isRotationLocked)
        {
            return -100.0f * player.GetInertia().Y * player.AngularVelocity.Y;
        }

        public Type On(in Inputs.AwayLadder input)
        {
            return onGround ? To<Running>() : To<Falling>();
        }
    }
}
