using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.UMLGenerator;

namespace Jomolith.App.State;

public interface IAppLogic : ILogicBlock<AppLogic.AppState>;

[Meta, LogicBlock(typeof(AppState), Diagram = true), ClassDiagram]
public partial class AppLogic : LogicBlock<AppLogic.AppState>, IAppLogic
{
    public override Transition GetInitialState()
    {
        return To<AppState.InMainMenu>();
    }
}
