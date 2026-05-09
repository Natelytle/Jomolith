using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Jomolith.Play.Player.Domain;

namespace Jomolith.Play.Player.Humanoid.State;

public interface IPlayerLogic : ILogicBlock<PlayerLogic.PlayerState>;

[Meta]
[LogicBlock(typeof(PlayerState), Diagram = true)]
public partial class PlayerLogic : LogicBlock<PlayerLogic.PlayerState>, IPlayerLogic
{
    private AutoValue<float>.Binding? opacityBinding;

    public override Transition GetInitialState()
    {
        return To<PlayerState.Disabled>();
    }

    public override void OnStart()
    {
        IPlayerRepo playerRepo = Get<IPlayerRepo>();

        opacityBinding = playerRepo.AvatarOpacity.Bind()
            .OnValue(opacity => Context.Output(new Output.Visual.SetTransparency(opacity)));
    }

    public override void OnStop()
    {
        opacityBinding?.Dispose();
    }
}
