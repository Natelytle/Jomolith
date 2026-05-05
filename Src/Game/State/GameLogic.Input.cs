namespace Jomolith.Game.State;

public partial class GameLogic
{
    public static class Input
    {
        public readonly record struct MainMenuRequested;

        public readonly record struct LoadTower;

        public readonly record struct TowerLoaded;

        public readonly record struct PlayTower;

        public readonly record struct ExitTower;

        public readonly record struct RestartTower;
    }
}
