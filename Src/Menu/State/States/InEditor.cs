using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State.States;

public partial record MenuState
{
    [Meta]
    public partial record InEditor : MenuState
    {
        public InEditor()
        {
            this.OnEnter(() =>
            {
                Push();
                Output(new MenuLogic.Outputs.ShowEditor());
            });
        }
    }
}
