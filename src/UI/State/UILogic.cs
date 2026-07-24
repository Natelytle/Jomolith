
using Chickensoft.LogicBlocks;

namespace Jomolith.UI.State;

public class UILogic : LogicBlock
{
    public UILogic()
    {
        Set(new UIState.MainMenu());
        Set(new UIState.TowerSelect());
        Set(new UIState.Settings());
        Set(new UIState.Play());
        Set(new UIState.ExitPromptOpen());
    }
}
