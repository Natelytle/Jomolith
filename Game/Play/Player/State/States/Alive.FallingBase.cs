using Chickensoft.Introspection;

namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public partial record PlayerState
    {
        [Meta]
        public abstract partial record FallingBase : Alive, IGet<Input.HitFloor>
        {
            protected override float MaxForce => 143.0f;
            protected override float Gain => 150.0f;
            protected override float BalanceKp => 5000.0f;
            protected override float BalanceKd => 100.0f;

            public virtual Transition On(in Input.HitFloor input)
            {
                return To<Landed>();
            }
        }
    }
}