using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State.States;

public partial record MenuState
{
    [Meta]
    public partial record InSettings : MenuState
    {
        public InSettings()
        {
            this.OnEnter(() =>
            {
                Push();
                Output(new MenuLogic.Outputs.ShowSettingsMenu());
            });
        }
    }
}
