using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Camera.State;

public interface ICameraLogic : ILogicBlock<CameraLogic.CameraState>;

[Meta, LogicBlock(typeof(CameraState), Diagram = true)]
public partial class CameraLogic : LogicBlock<CameraLogic.CameraState>, ICameraLogic
{
    public override Transition GetInitialState()
    {
        return To<CameraState.Unlocked>();
    }
}