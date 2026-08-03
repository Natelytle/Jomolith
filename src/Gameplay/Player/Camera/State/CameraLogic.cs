using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Gameplay.Player.Camera.State.States;

namespace Jomolith.Gameplay.Player.Camera.State;

public interface ICameraLogic : ILogicBlock;

[Meta]
public partial class CameraLogic : LogicBlock, ICameraLogic
{
    public CameraLogic()
    {
        Set(new CameraState.Unlocked());
        Set(new CameraState.ShiftLock());
        Set(new CameraState.FirstPerson());
        Set(new CameraState.ShiftLockFirstPerson());
    }
}
