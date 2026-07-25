using Chickensoft.GodotNodeInterfaces;
using Jomolith.Menu.State;

namespace Jomolith.Menu;

public interface IScreen : IControl
{
    bool ShowFooter { get; }

    // When the screen is pushed and popped
    void OnEnter();
    void OnExit();
}
