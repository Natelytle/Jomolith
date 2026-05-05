using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.Player.Camera.PerspectiveState;

public interface IPerspectiveLogic : ILogicBlock<PerspectiveLogic.PerspectiveState>;

[Meta]
[LogicBlock(typeof(PerspectiveState), Diagram = true)]
public partial class PerspectiveLogic : LogicBlock<PerspectiveLogic.PerspectiveState>, IPerspectiveLogic
{
    public override Transition GetInitialState()
    {
        return To<PerspectiveState.ThirdPerson>();
    }
}
