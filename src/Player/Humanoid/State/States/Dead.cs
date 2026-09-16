using Chickensoft.Introspection;

namespace Jomolith.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Dead : PlayerState
    {
    }
}
