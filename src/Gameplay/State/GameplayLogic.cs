using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;
using Jomolith.Gameplay.Domain;

namespace Jomolith.Gameplay.State;

public interface IGameplayLogic : ILogicBlock;

[Meta]
public partial class GameplayLogic : LogicBlock, IGameplayLogic
{
    public GameplayLogic()
    {
        Set(new GameplayData());
        Set(new GameplayState.Unloaded());
        Set(new GameplayState.Loading());
        Set(new GameplayState.Playing());
        Set(new GameplayState.Paused());
        Set(new GameplayState.Quit());
    }

    public override IEnumerable<IDisposable> OnStartSubscriptions()
    {
        yield return Get<IAppRepo>().AutoChannel.Bind()
            .On((in IAppRepo.EnteringTower o) => (State as GameplayState.Unloaded)?.OnTowerEntered(o.Tower));

        var gameplayRepo = Get<IGameplayRepo>();

        yield return gameplayRepo.IsMouseCaptured.Bind()
            .OnValue(isMouseCaptured => State?.Output(new GameplayState.Output.SetMouseCaptureMode(isMouseCaptured)));
    }
}
