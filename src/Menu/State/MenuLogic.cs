
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State;

public interface IMenuLogic : ILogicBlock;

[Meta]
public partial class MenuLogic : LogicBlock, IMenuLogic
{
    public MenuLogic()
    {
        Set(new MenuState.MainMenu());
        Set(new MenuState.TowerSelect());
        Set(new MenuState.Settings());
        Set(new MenuState.ExitPromptOpen());
    }
}
