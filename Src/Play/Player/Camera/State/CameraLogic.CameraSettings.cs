namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
{
    public record CameraSettings
    {
        public float Sensitivity { get; set; } = 1.0f;
    }
}