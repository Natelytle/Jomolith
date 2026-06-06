using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Jomolith.Menu.State.States;

namespace Jomolith.Menu.State;

public interface IMenuLogic : ILogicBlock;

[Meta]
public partial class MenuLogic : AutoBlock, IMenuLogic
{
    public MenuLogic()
    {
        Preallocate<MenuState>();
    }
}
