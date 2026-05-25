using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.App.State;

public interface IAppLogic : ILogicBlock<AppLogic.AppState>;

[Meta]
[LogicBlock(typeof(AppState), Diagram = true)]
public partial class AppLogic : LogicBlock<AppLogic.AppState>, IAppLogic
{
    public override Transition GetInitialState()
    {
        return To<AppState.InMainMenu>();
    }
}
