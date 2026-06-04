using Jomolith.Tower.Core;

namespace Jomolith.Tower.State;

public partial class TowerLogic
{
    public static class Input
    {
        public readonly record struct Default;

        public readonly record struct LoadTower(ITowerModel TowerModel);

        public readonly record struct UnloadTower;
    }
}
