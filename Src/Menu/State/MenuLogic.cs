using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State;

public interface IMenuLogic : ILogicBlock<MenuLogic.MenuState>;

[Meta, Id("menu_logic")]
[LogicBlock(typeof(MenuState), Diagram = true)]
public partial class MenuLogic : LogicBlock<MenuLogic.MenuState>, IMenuLogic
{
    public override Transition GetInitialState() => To<MenuState.Default>();
}
