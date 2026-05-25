using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.App.State;

public partial class AppLogic
{
    public partial record AppState
    {
        [Meta]
        public partial record LoadingTower : AppState, IGet<Input.TowerLoaded>
        {
            public LoadingTower()
            {
                this.OnEnter(() =>
                {
                    Output(new Output.StartLoadingTower());
                });
            }

            public Transition On(in Input.TowerLoaded input)
            {
                return To<InTower>();
            }
        }
    }
}
