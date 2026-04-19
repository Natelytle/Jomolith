namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    public record Data
    {
        public bool ShouldLoadExistingGame { get; set; }
    }
}