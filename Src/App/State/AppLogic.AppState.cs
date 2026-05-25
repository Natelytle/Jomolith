using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.App.State;

public partial class AppLogic
{
    [Meta]
    public abstract partial record AppState : StateLogic<AppState>;
}
