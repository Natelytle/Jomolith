using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using static Jomolith.Gameplay.Player.Camera.State.CameraLogic;

namespace Jomolith.Gameplay.Player.Camera.State.States;

public abstract partial record CameraState
{
    [Meta]
    public partial record Unlocked : CameraState, IGet<Inputs.ToggleShiftLock>, IGet<Inputs.FirstPersonEntered>
    {
        public Unlocked()
        {
            this.OnEnter(() =>
            {
                Output(new Outputs.SetCameraLocked(false));
                Output(new Outputs.SetPlayerLocked(false));
            });
        }

        public Type On(in Inputs.ToggleShiftLock input)
        {
            return To<ShiftLock>();
        }

        public Type On(in Inputs.FirstPersonEntered input)
        {
            return To<FirstPerson>();
        }
    }
}
