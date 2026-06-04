using Chickensoft.Introspection;

namespace Jomolith.Menu.State;

public partial class MenuLogic
{
    public partial record MenuState
    {
        [Meta, Id("menu_logic_state_default")]
        public partial record Default : MenuState
        {

        }
    }
}
