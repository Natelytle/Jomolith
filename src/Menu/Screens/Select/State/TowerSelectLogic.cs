using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.Screens.Select.State;

public interface ITowerSelectLogic : ILogicBlock;

[Meta]
public partial class TowerSelectLogic : LogicBlock, ITowerSelectLogic
{
    public TowerSelectLogic()
    {
        Set(new TowerSelectionData());
        Set(new TowerSelectState.Loading());
        Set(new TowerSelectState.Browsing());
    }
}
