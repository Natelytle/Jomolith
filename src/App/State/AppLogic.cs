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
        yield return Get<IAppRepo>().AutoChannel.Bind()
            .On((in IAppRepo.EnteringTower o) => (State as AppState.InMenus)?.OnTowerEntered(o.Tower))
            .On((in IAppRepo.ExitingTower _) => (State as AppState.InGameplay)?.OnTowerExited());
    }
}
