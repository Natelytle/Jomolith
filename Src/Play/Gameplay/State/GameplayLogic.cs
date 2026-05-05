using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.Sync.Primitives;
using Jomolith.Play.Gameplay.Domain;

namespace Jomolith.Play.Gameplay.State;

public interface IGameplayLogic : ILogicBlock<GameplayLogic.GameplayState>;

[Meta]
[LogicBlock(typeof(GameplayState), Diagram = true)]
public partial class GameplayLogic : LogicBlock<GameplayLogic.GameplayState>, IGameplayLogic
{
    private AutoValue<bool>.Binding? mouseCapturedBinding;
    private AutoValue<bool>.Binding? gamePausedBinding;

    public override Transition GetInitialState()
    {
        return To<GameplayState>();
    }

    public override void OnStart()
    {
        IGameplayRepo gameplayRepo = Get<IGameplayRepo>();

        mouseCapturedBinding = gameplayRepo.IsMouseCaptured.Bind()
            .OnValue((isMouseCaptured) => Context.Output(new Output.SetMouseCaptureMode(isMouseCaptured)));

        gamePausedBinding = gameplayRepo.IsPaused.Bind()
            .OnValue((isPaused) => Context.Output(new Output.SetPaused(isPaused)));
    }

    public override void OnStop()
    {
        mouseCapturedBinding?.Dispose();
        gamePausedBinding?.Dispose();
    }
}
