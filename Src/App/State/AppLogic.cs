using System;
using System.Collections.Generic;
using Chickensoft.LogicBlocks;
using Chickensoft.LogicBlocks.Auto;
using Jomolith.App.Domain;
using Jomolith.App.State.States;

namespace Jomolith.App.State;

public interface IAppLogic : ILogicBlock;

public partial class AppLogic : AutoBlock, IAppLogic
{
    public AppLogic()
    {
        Preallocate<AppState>();
    }

    public override IEnumerable<IDisposable> OnStartSubscriptions()
    {
        yield return Get<IAppRepo>().AutoChannel.Bind()
            .On((in IAppRepo.TowerExited _) => (State as AppState.InTower)?.OnTowerExited());
    }
}
