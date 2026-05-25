using Chickensoft.Introspection;

namespace Jomolith.App.State;

public partial class AppLogic
{
    public partial record AppState
    {
        [Meta]
        public partial record ExitingTower : AppState, IGet<Input.MainMenuRequested>
        {
            public Transition On(in Input.MainMenuRequested input)
            {
                return To<InMainMenu>();
            }
        }
    }
}
