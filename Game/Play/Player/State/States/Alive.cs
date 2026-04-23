using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Game.Play.Player.Utils;
using Jomolith.Game.Play.Tower.Domain;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public abstract partial record Alive : PlayerState, 
            IGet<Input.PhysicsTick>, 
            IGet<Input.ComputeForces>, 
            IGet<Input.DesiredMovementVector>, 
            IGet<Input.SetTimer>
        {
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

            public Transition On(in Input.PhysicsTick input)
            {
                IHumanoid player = Get<IHumanoid>();
                ITowerRepo towerRepo = Get<ITowerRepo>();
                PlayerData playerData = Get<PlayerData>();

                Input(new Input.ComputeForces(input.Delta));

                Basis cameraBasis = towerRepo.CameraBasis.Value;
                Vector2 desiredMoveDirection = player.GetGlobalInputVector(cameraBasis);

                Input(new Input.DesiredMovementVector(desiredMoveDirection));

                // Raycasts
                FloorData floorData = player.GetFloorData(playerData.WasOnFloor);

                if (floorData.FloorFound)
                {
                    if (!playerData.WasOnFloor)
                        Input(new Input.HitFloor());

                    Input(new Input.OnFloor(floorData));
                }
                else if (playerData.WasOnFloor)
                {
                    Input(new Input.OffFloor());
                }

                playerData.WasOnFloor = floorData.FloorFound;
                playerData.FloorNormal = floorData.FloorNormal;
                playerData.FloorPosition = floorData.FloorPosition;
                playerData.FloorVelocity = floorData.FloorVelocity;

                // Set player statistics
                playerData.PlayerHeading = new Plane(Vector3.Up).Project(-player.Basis.Z).Normalized();

                // Check & update our timer
                if (playerData.Timer > 0 && playerData.Timer - input.Delta < 0)
                    Input(new Input.TimerUp());

                playerData.Timer -= input.Delta;

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
                Vector3 correctionVector = targetMovementVector - new Vector3(player.LinearVelocity.X, 0, player.LinearVelocity.Z);
                correctionVector = correctionVector.Normalized() * Math.Min(MaxForce, Gain * correctionVector.Length());
                Vector3 desiredForce = correctionVector * player.Mass;

                float angleToDesiredDirection = playerData.PlayerHeading.SignedAngleTo(targetMovementVector, Vector3.Up);
                float desiredRotationalVelocity = 8.0f * angleToDesiredDirection;
                float desiredTorque = 100.0f * player.GetInertia().Y * (desiredRotationalVelocity - player.AngularVelocity.Y);
                desiredTorque = Mathf.Clamp(desiredTorque, -1e5f, 1e5f);

                Output(new Output.ApplyForce(desiredForce, Vector3.Up * desiredTorque));

                Vector2 floorVelocity = new Vector2(player.LinearVelocity.X, player.LinearVelocity.Z);

                Output(new Output.FloorVelocityChanged(floorVelocity));

                if (floorVelocity.Length() > 0.5)
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