namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
    public record CameraSettings
    {
        public float Sensitivity { get; set; } = 1.0f;
    }
}