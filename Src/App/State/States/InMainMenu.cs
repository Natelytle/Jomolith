using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;

namespace Jomolith.App.State;

public partial class AppLogic
{
    public partial record AppState
    {
        [Meta]
        public partial record InMainMenu : AppState, IGet<Input.PlayTower>
        {
            public Transition On(in Input.PlayTower input)
            {
                return To<LoadingTower>();
            }
        }
    }
}
