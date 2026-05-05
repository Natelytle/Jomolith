using Chickensoft.Introspection;

namespace Jomolith.Play.Player.Humanoid.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Dead : PlayerState
        {
        }
    }
}
