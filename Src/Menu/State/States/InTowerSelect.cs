using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State.States;

public partial record MenuState
{
    [Meta]
    public partial record InTowerSelect : MenuState
    {
        public InTowerSelect()
        {
            this.OnEnter(() =>
            {
                Push();
                Output(new MenuLogic.Outputs.ShowTowerSelect());
            });
        }
    }
}
