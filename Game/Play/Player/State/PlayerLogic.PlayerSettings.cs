namespace Jomolith.Game.Play.Player.State;

public partial class PlayerLogic
{
    /// <summary>Player settings.</summary>
    /// <param name="GravityScale">Player gravity multiplier (x196.2 studs/s^2).</param>
    /// <param name="MoveSpeed">Player speed (studs/sec).</param>
    /// <param name="JumpPower">Player jump power (studs/sec)</param>
    public record PlayerSettings(
        float GravityScale,
        float MoveSpeed,
        float JumpPower
    );
}