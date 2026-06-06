using Chickensoft.Introspection;

namespace Jomolith.Menu.State.States;

public partial record MenuState
{
    [Meta]
    public partial record Default : MenuState
    {

    }
}
