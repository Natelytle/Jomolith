using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Camera.LockModeState;

public interface ILockModeLogic : ILogicBlock<LockModeLogic.LockModeState>;

[Meta]
[LogicBlock(typeof(LockModeState), Diagram = true)]
public partial class LockModeLogic : LogicBlock<LockModeLogic.LockModeState>, ILockModeLogic
{
    public override Transition GetInitialState()
    {
        return To<LockModeState.FreeCam>();
    }
}