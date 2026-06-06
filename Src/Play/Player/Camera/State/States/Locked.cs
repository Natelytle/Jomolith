using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using static Jomolith.Play.Player.Camera.State.CameraLogic;

namespace Jomolith.Play.Player.Camera.State.States;

public abstract partial record CameraState
{
    [Meta]
    public abstract partial record Locked : CameraState
    {
        protected Locked()
        {
            this.OnEnter(() =>
            {
                Output(new Outputs.SetCameraLocked(true));
                Output(new Outputs.SetPlayerLocked(true));
            });
        }
    }

    [Meta]
    public partial record ShiftLock : Locked, IGet<Inputs.ToggleShiftLock>, IGet<Inputs.FirstPersonEntered>
    {
        private static readonly Vector3 ShiftLockOffset = new Vector3(1.75f, 0, 0);

        public ShiftLock()
        {
            this.OnEnter(() => Output(new Outputs.OffsetChanged(ShiftLockOffset)));
            this.OnExit(() => Output(new Outputs.OffsetChanged(Vector3.Zero)));
        }

        public Type On(in Inputs.ToggleShiftLock input)
        {
            return To<Unlocked>();
        }

        public Type On(in Inputs.FirstPersonEntered input)
        {
            return To<ShiftLockFirstPerson>();
        }
    }

    [Meta]
    public partial record FirstPerson : Locked, IGet<Inputs.ToggleShiftLock>, IGet<Inputs.FirstPersonExited>
    {
        public virtual Type On(in Inputs.ToggleShiftLock input)
        {
            return To<ShiftLockFirstPerson>();
        }

        public virtual Type On(in Inputs.FirstPersonExited input)
        {
            return To<Unlocked>();
        }
    }

    [Meta]
    public partial record ShiftLockFirstPerson : FirstPerson
    {
        public override Type On(in Inputs.ToggleShiftLock input)
        {
            return To<FirstPerson>();
        }

        public override Type On(in Inputs.FirstPersonExited input)
        {
            return To<ShiftLock>();
        }
    }
}
