using Chickensoft.GodotNodeInterfaces;
using Jomolith.UI.State;

namespace Jomolith.UI;

public interface IScreen : IControl
{
    // When the screen is pushed and popped
    void OnEnter(UILogic logic);
    void OnExit();
}
