namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public static class Output
    {
        public readonly record struct ShowMainMenu;
        
        public readonly record struct StartLoadingTower;

        public readonly record struct EnterTower;

        public readonly record struct UnloadCurrentTower;
    }
}