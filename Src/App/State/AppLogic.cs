using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Jomolith.App.State.States;

namespace Jomolith.App.State;

public interface IAppLogic : ILogicBlock;

public partial class AppLogic : AutoBlock, IAppLogic
{
    public AppLogic()
    {
        Preallocate<AppState>();
    }
}
