using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.App.State;

[Meta]
public abstract partial record AppState : LogicBlockState
{
    public static class Input
    {
        public readonly record struct ToGameplay(TowerModel Tower);
    }

    public static class Output
    {
        public readonly record struct GameplayEntered;
    }



    [Meta]
    public partial record InMenus : AppState
    {

    }

    public partial record InGameplay : AppState
    {

    }
}
