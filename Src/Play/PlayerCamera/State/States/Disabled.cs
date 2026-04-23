using Chickensoft.Introspection;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
    public partial record CameraState
    {
        [Meta]
        public partial record Disabled : CameraState
        {
        }
    }
}