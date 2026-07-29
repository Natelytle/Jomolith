using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.App.State;

public interface IAppLogic : ILogicBlock;

[Meta]
public partial class AppLogic : LogicBlock, IAppLogic
{

}
