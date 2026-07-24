using Chickensoft.GodotNodeInterfaces;
using Jomolith.Menu.State;

namespace Jomolith.Menu;

public interface IScreen : IControl
{
    // When the screen is pushed and popped
    void OnEnter();
    void OnExit();
}
