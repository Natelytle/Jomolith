using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State;

public partial class MenuLogic
{
    [Meta]
    public abstract partial record MenuState : StateLogic<MenuState>;
}
