using Chickensoft.Introspection;

namespace Jomolith.Play.Player.Humanoid.State.States;

public partial record PlayerState
{
    [Meta]
    public partial record Dead : PlayerState
    {
    }
}
