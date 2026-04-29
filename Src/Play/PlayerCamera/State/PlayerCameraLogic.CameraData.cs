using Godot;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
    public record CameraData
    {
        public required float DesiredZoomLength;

        public bool ShouldPan => RightClickPressed || CameraLocked;

        public required bool RightClickPressed;

        public required bool CameraLocked;
    }
}