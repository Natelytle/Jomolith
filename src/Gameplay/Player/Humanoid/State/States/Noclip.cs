
using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Gameplay.Player.Domain;
using static Jomolith.Gameplay.Player.Humanoid.State.PlayerLogic;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Noclip : PlayerState,
        IGet<Inputs.ToggleNoclip>
    {
        public Noclip()
        {
            this.OnEnter(() =>
            {
                Output(new Outputs.SetFrozen(true));
                Output(new Outputs.Animations.Disabled());
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

            Output(new Outputs.SetTransform(newTransform));
        }

        public Type On(in Inputs.ToggleNoclip input)
        {
            return To<Falling>();
        }
    }
}
