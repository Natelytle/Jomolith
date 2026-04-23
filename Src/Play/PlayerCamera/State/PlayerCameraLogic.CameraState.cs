using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
  [Meta]
  public abstract partial record CameraState : StateLogic<CameraState>;
}