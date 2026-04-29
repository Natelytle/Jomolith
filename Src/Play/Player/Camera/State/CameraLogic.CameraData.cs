using Godot;

namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
{
    public record CameraData
    {
        public required float DesiredZoomLength;

        public bool ShouldPan => RightClickPressed || CameraLocked;

        public required bool RightClickPressed;

        public required bool CameraLocked;
    }
}