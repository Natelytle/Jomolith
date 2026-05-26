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
        public partial record Noclip : PlayerState,
            IGet<Input.PhysicsTickAlive>,
            IGet<Input.Jump>,
            IGet<Input.ToggleNoclip>
        {
            private Vector3 noclipDirection = Vector3.Zero;

            public Noclip()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.SetFrozen(true));
                    Output(new Output.Animations.Disabled());
                });
            }

            public Transition On(in Input.PhysicsTickAlive input)
            {
                IHumanoid player = Get<IHumanoid>();
                IPlayerRepo playerRepo = Get<IPlayerRepo>();

                Vector2 unadjustedMovementVector = player.GetUnadjustedInputVector();
                noclipDirection += new Vector3(unadjustedMovementVector.X, 0, unadjustedMovementVector.Y);

                // Rotate around the head
                player.GlobalPosition += player.GlobalRootPosition + player.GlobalBasis.Y * 1.5f - player.GlobalPosition;
                player.GlobalRotation = playerRepo.CameraBasis.Value.GetEuler();
                player.GlobalPosition -= player.GlobalRootPosition + player.GlobalBasis.Y * 1.5f - player.GlobalPosition;

                // Actually move
                player.GlobalPosition += player.Basis * noclipDirection.Normalized() * (float)input.Delta * 32f;

                noclipDirection = Vector3.Zero;

                return ToSelf();
            }

            public Transition On(in Input.Jump input)
            {
                noclipDirection += Vector3.Up;

                return ToSelf();
            }

            public Transition On(in Input.ToggleNoclip input)
            {
                return To<Falling>();
            }
        }
    }
}
