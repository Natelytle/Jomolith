namespace Jomolith.Game.State;

public partial class GameLogic
{
    public record Data
    {
        public bool ShouldLoadExistingGame { get; set; }
    }
}