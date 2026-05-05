namespace Jomolith.Game.State;

public partial class GameLogic
{
    public static class Output
    {
        public readonly record struct ShowMainMenu;

        public readonly record struct StartLoadingTower;

        public readonly record struct EnterTower;

        public readonly record struct UnloadCurrentTower;
    }
}
