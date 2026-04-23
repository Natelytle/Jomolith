using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.PlayerCamera.State;

public interface IPlayerCameraLogic : ILogicBlock<PlayerCameraLogic.CameraState>;

[Meta]
[LogicBlock(typeof(CameraState), Diagram = true)]
public partial class PlayerCameraLogic : LogicBlock<PlayerCameraLogic.CameraState>, IPlayerCameraLogic
{
    public override Transition GetInitialState()
    {
        return To<CameraState.Disabled>();
    }
}