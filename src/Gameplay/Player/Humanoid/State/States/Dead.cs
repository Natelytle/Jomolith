using Chickensoft.Introspection;

namespace Jomolith.Gameplay.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Dead : PlayerState
    {
    }
}
