using Godot;

namespace Jomolith.Play.Player.Camera.State;

public partial class CameraLogic
{
    public record CameraData
    {
        public required float DesiredZoomLength;

        public bool CameraLocked => CameraLockedRightClick || CameraLockedState;

        public required bool CameraLockedRightClick;

        public required bool CameraLockedState;
    }
}
