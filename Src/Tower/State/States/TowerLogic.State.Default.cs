namespace Jomolith.Tower.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class TowerLogic
{
    public partial record TowerState
    {
        [Meta, Id("tower_logic_state_default")]
        public partial record Default : TowerState,
        IGet<Input.Default>
        {
            public Default()
            {
                this.OnEnter(() =>
                {
                    // EDIT ME
                });

                this.OnExit(() =>
                {
                    // EDIT ME
                });
            }

            public Transition On(in Input.Default input)
            {
                // EDIT ME
                return ToSelf();
            }
        }
    }

}
