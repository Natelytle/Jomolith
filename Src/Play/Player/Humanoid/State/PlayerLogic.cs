using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Sync.Primitives;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Humanoid.State.States;

namespace Jomolith.Play.Player.Humanoid.State;

public interface IPlayerLogic : IAutoLogicBlock;

[Meta]
public partial class PlayerLogic : AutoBlock, IPlayerLogic
{
    public PlayerLogic()
    {
        Preallocate<PlayerState>();
    }

    private AutoValue<float>.Binding? opacityBinding;

    public override void OnStart()
    {
        IPlayerRepo playerRepo = Get<IPlayerRepo>();

        opacityBinding = playerRepo.AvatarOpacity.Bind()
            .OnValue(opacity => State?.Output(new Outputs.Visual.SetTransparency(opacity)));
    }

    public override void OnStop()
    {
        opacityBinding?.Dispose();
    }
}
