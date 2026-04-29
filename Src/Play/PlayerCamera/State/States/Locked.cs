using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
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
                    Get<CameraData>().CameraLocked = true;
                    Get<ITowerRepo>().SetIsPlayerRotationLocked(true);
                });
            }
        }

        [Meta]
        public partial record ShiftLock : Locked, IGet<Input.ToggleShiftLock>, IGet<Input.FirstPersonEntered>
        {
            private static readonly Vector3 ShiftLockOffset = new Vector3(1.75f, 0, 0);

            public ShiftLock()
            {
                this.OnEnter(() => Output(new Output.OffsetChanged(ShiftLockOffset)));
                this.OnExit(() => Output(new Output.OffsetChanged(Vector3.Zero)));
            }

            public Transition On(in Input.ToggleShiftLock input)
            {
                Output(new Output.ShiftLockExited(FirstPerson: false));

                return To<Unlocked>();
            }

            public Transition On(in Input.FirstPersonEntered input)
            {
                Output(new Output.FirstPersonEntered(ShiftLock: true));

                return To<ShiftLockFirstPerson>();
            }
        }

        [Meta]
        public partial record FirstPerson : Locked, IGet<Input.ToggleShiftLock>, IGet<Input.FirstPersonExited>
        {
            public virtual Transition On(in Input.ToggleShiftLock input)
            {
                Output(new Output.ShiftLockEntered(FirstPerson: true));

                return To<ShiftLockFirstPerson>();
            }

            public virtual Transition On(in Input.FirstPersonExited input)
            {
                Output(new Output.FirstPersonExited(ShiftLock: false));

                return To<Unlocked>();
            }
        }

        // Little bit duplicate-y, but the code is easier to write this way, and it's small anyway.
        [Meta]
        public partial record ShiftLockFirstPerson : FirstPerson
        {
            public override Transition On(in Input.ToggleShiftLock input)
            {
                Output(new Output.ShiftLockExited(FirstPerson: true));

                return To<FirstPerson>();
            }

            public override Transition On(in Input.FirstPersonExited input)
            {
                Output(new Output.FirstPersonExited(ShiftLock: true));

                return To<ShiftLock>();
            }
        }
    }
}