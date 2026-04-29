using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
    public abstract partial record CameraState
    {
        [Meta]
        public partial record Unlocked : CameraState, IGet<Input.ToggleShiftLock>, IGet<Input.FirstPersonEntered>
        {
            public Unlocked()
            {
                this.OnEnter(() =>
                {
                    Get<CameraData>().CameraLocked = false;
                    Get<ITowerRepo>().SetIsPlayerRotationLocked(false);
                });
            }

            public Transition On(in Input.ToggleShiftLock input)
            {
                Output(new Output.ShiftLockEntered(FirstPerson: false));

                return To<ShiftLock>();
            }

            public Transition On(in Input.FirstPersonEntered input)
            {
                Output(new Output.FirstPersonEntered(ShiftLock: false));

                return To<FirstPerson>();
            }
        }
    }
}
