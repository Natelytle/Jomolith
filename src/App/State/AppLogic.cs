using System;
using System.Collections.Generic;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.App.Domain;

namespace Jomolith.App.State;

public interface IAppLogic : ILogicBlock;

[Meta]
public partial class AppLogic : LogicBlock, IAppLogic
{
    public AppLogic()
    {
        Set(new AppState.InMenus());
        Set(new AppState.InGameplay());
    }

    public override IEnumerable<IDisposable> OnStartSubscriptions()
    {
        // Ensure the state makes sense for the input by casting
        yield return Get<IAppRepo>().AutoChannel.Bind()
            .On((in IAppRepo.EnteringTower _) => (State as AppState.InMenus)?.OnEnteringTower())
            .On((in IAppRepo.ExitingTower _) => (State as AppState.InGameplay)?.OnExitingTower());
    }
}
