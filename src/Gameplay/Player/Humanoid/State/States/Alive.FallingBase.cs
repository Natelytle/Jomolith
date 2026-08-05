using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public abstract partial record FallingBase : Alive, IGet<Inputs.HitFloor>
    {
        private const float falling_friction = 0.14f;
        private const float running_friction = 0.5f;

        protected FallingBase()
        {
            this.OnEnter(() => Output(new Outputs.SetFriction(falling_friction)));

            this.OnExit(() => Output(new Outputs.SetFriction(running_friction)));
        }

        protected override float MaxForce => 143.0f;
        protected override float Gain => 150.0f;
        protected override float BalanceKp => 5000.0f;
        protected override float BalanceKd => 100.0f;

        private const float torque_angle_max = 1.0f;

        public virtual Type On(in Inputs.HitFloor input)
        {
            if (input.VerticalVelocity <= 0)
                return To<Landed>();

            return ToSelf();
        }

        protected override float ComputeTorque(Vector3 movementVector, PlayerData playerData, IHumanoid player, bool isRotationLocked)
        {
            // We have torque even in shift lock in the falling state in order to match preestablished behaviour.
            float angleDifference = playerData.PlayerHeading.SignedAngleTo(movementVector, Vector3.Up);
            float desiredAngVel = 8.0f * Math.Min(Math.Abs(angleDifference), torque_angle_max) * Math.Sign(angleDifference);

            return 100.0f * player.GetInertia().Y * (desiredAngVel - player.AngularVelocity.Y);
        }
    }
}
