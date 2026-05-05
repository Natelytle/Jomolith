using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Player.Domain;

namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
{
    public abstract partial record CameraState
    {
        [Meta]
        public abstract partial record Locked : CameraState
        {
            public Locked()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.SetCameraLocked(true));
                    Output(new Output.SetPlayerLocked(true));
                });
            }
        }

        [Meta]
        public partial record ShiftLock : Locked, IGet<Input.ToggleShiftLock>, IGet<Input.FirstPersonEntered>
        {
            private static readonly Vector3 shift_lock_offset = new(1.75f, 0, 0);

            public ShiftLock()
            {
                this.OnEnter(() => Output(new Output.OffsetChanged(shift_lock_offset)));
                this.OnExit(() => Output(new Output.OffsetChanged(Vector3.Zero)));
            }

            public Transition On(in Input.ToggleShiftLock input)
            {
                return To<Unlocked>();
            }

            public Transition On(in Input.FirstPersonEntered input)
            {
                return To<ShiftLockFirstPerson>();
            }
        }

        [Meta]
        public partial record FirstPerson : Locked, IGet<Input.ToggleShiftLock>, IGet<Input.FirstPersonExited>
        {
            public virtual Transition On(in Input.ToggleShiftLock input)
            {
                return To<ShiftLockFirstPerson>();
            }

            public virtual Transition On(in Input.FirstPersonExited input)
            {
                return To<Unlocked>();
            }
        }

        [Meta]
        public partial record ShiftLockFirstPerson : FirstPerson
        {
            public override Transition On(in Input.ToggleShiftLock input)
            {
                return To<FirstPerson>();
            }

            public override Transition On(in Input.FirstPersonExited input)
            {
                return To<ShiftLock>();
            }
        }
    }
}
