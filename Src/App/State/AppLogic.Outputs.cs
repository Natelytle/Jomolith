namespace Jomolith.App.State;

public partial class AppLogic
{
    public static class Outputs
    {
        public readonly record struct ShowMainMenu;

        public readonly record struct StartLoadingTower;

        public readonly record struct EnterTower;

        public readonly record struct UnloadCurrentTower;
    }
}
