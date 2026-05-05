using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Play.Player.Domain;

namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
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
                    Output(new Output.SetCameraLocked(false));
                    Output(new Output.SetPlayerLocked(false));
                });
            }

            public Transition On(in Input.ToggleShiftLock input)
            {
                return To<ShiftLock>();
            }

            public Transition On(in Input.FirstPersonEntered input)
            {
                return To<FirstPerson>();
            }
        }
    }
}
