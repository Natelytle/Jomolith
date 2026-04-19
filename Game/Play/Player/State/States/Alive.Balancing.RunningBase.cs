using Chickensoft.Introspection;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record RunningBase : Balancing
        {
        }
    }
}