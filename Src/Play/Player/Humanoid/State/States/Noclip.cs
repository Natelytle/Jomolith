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
            IGet<Input.ToggleNoclip>
        {
            public Noclip()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.SetFrozen(true));
                    Output(new Output.Animations.Disabled());
                });
            }

            protected override void ProcessPhysics(double delta)
            {
                IHumanoid player = Get<IHumanoid>();
                IPlayerRepo playerRepo = Get<IPlayerRepo>();

                Vector3 desiredMoveDirection = player.GetNoclipInputVector(playerRepo.CameraBasis.Value);

                // Rotate around the head
                Transform3D newTransform = player.GlobalTransform;

                // Move the origin up to where the head was, so that when we rotate around the origin we rotate around our head position.
                // The center of the head is 4.5 studs above the origin.
                newTransform.Origin += newTransform.Basis.Y * 4.5f;
                newTransform.Basis = playerRepo.CameraBasis.Value;

                // Move it back down to where it *should* be after the head was moved
                newTransform.Origin -= newTransform.Basis.Y * 4.5f;

                // Actually move
                newTransform.Origin += desiredMoveDirection * (float)delta * 32f;

                Output(new Output.SetTransform(newTransform));
            }

            public Transition On(in Input.ToggleNoclip input)
            {
                return To<Falling>();
            }
        }
    }
}
