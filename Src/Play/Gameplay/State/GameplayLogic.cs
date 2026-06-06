using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Sync.Primitives;
using Jomolith.Play.Gameplay.Domain;
using Jomolith.Play.Gameplay.State.States;

namespace Jomolith.Play.Gameplay.State;

public interface IGameplayLogic : ILogicBlock;

[Meta]
public partial class GameplayLogic : AutoBlock, IGameplayLogic
{
    private AutoValue<bool>.Binding? mouseCapturedBinding;
    private AutoValue<bool>.Binding? gamePausedBinding;

    public GameplayLogic()
    {
        Preallocate<GameplayState>();
    }

    public override void OnStart()
    {
        IGameplayRepo gameplayRepo = Get<IGameplayRepo>();

        mouseCapturedBinding = gameplayRepo.IsMouseCaptured.Bind()
            .OnValue(isMouseCaptured => State?.Output(new Outputs.SetMouseCaptureMode(isMouseCaptured)));

        gamePausedBinding = gameplayRepo.IsPaused.Bind()
            .OnValue(isPaused => State?.Output(new Outputs.SetPaused(isPaused)));
    }

    public override void OnStop()
    {
        mouseCapturedBinding?.Dispose();
        gamePausedBinding?.Dispose();
    }
}
