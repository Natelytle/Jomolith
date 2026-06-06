using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Jomolith.Play.Player.Camera.State.States;

namespace Jomolith.Play.Player.Camera.State;

public interface ICameraLogic : ILogicBlock;

[Meta]
public partial class CameraLogic : AutoBlock, ICameraLogic
{
    public CameraLogic()
    {
        Preallocate<CameraState>();
    }
}
