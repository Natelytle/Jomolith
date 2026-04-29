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
                    Get<CameraData>().CameraLocked = false;
                    Get<IPlayerRepo>().SetIsPlayerRotationLocked(false);
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
