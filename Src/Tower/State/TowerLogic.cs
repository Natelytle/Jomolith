using Chickensoft.LogicBlocks.Auto;
using Chickensoft.Sync.Primitives;
using Jomolith.Tower.Core;
using Jomolith.Tower.Domain;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Jomolith.Tower.State.States;

namespace Jomolith.Tower.State;

public interface ITowerLogic : ILogicBlock;

[Meta]
public partial class TowerLogic : AutoBlock, ITowerLogic
{
    private AutoValue<ITowerModel?>.Binding? currentTowerBinding;

    public TowerLogic()
    {
        Preallocate<TowerState>();
    }

    public override void OnStart()
    {
        ITowerRepo towerRepo = Get<ITowerRepo>();

        currentTowerBinding = towerRepo.CurrentTower.Bind().OnValue((tower) =>
        {
            if (tower is not null)
                State?.Input(new Inputs.LoadTower(tower));
            else
                State?.Input(new Inputs.UnloadTower());
        });
    }

    public override void OnStop()
    {
        currentTowerBinding?.Dispose();
    }
}
