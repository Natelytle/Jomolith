using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.Screens.Settings.State;

public interface ISettingsLogic : ILogicBlock;

[Meta]
public partial class SettingsLogic : LogicBlock, ISettingsLogic
{
    public SettingsLogic()
    {
        Set(new SettingsData());
        Set(new SettingsState.Loading());
        Set(new SettingsState.Editing());
        Set(new SettingsState.RebindingAction());
    }
}
