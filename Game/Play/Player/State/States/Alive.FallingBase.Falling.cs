using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public partial record Falling : FallingBase
        {
            public Falling()
            {
                this.OnEnter(() => Output(new Output.Animations.Fall()));
            }
        }
    }
}