using Chickensoft.Sync.Primitives;
using Chickensoft.UMLGenerator;
using Jomolith.Tower.Core;
using Jomolith.Tower.Domain;

namespace Jomolith.Tower.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public interface ITowerLogic : ILogicBlock<TowerLogic.State>;

[Meta, Id("tower_logic")]
[LogicBlock(typeof(State), Diagram = true), ClassDiagram]
public partial class TowerLogic : LogicBlock<TowerLogic.State>, ITowerLogic
{
    private AutoValue<ITowerModel?>.Binding? currentTowerBinding;

    public override Transition GetInitialState() => To<State.Default>();

    public override void OnStart()
    {
        ITowerRepo towerRepo = Get<ITowerRepo>();

        currentTowerBinding = towerRepo.CurrentTower.Bind().OnValue((tower) =>
        {
            if (tower is not null)
                Context.Input(new Input.LoadTower(tower));
            else
                Context.Input(new Input.UnloadTower());
        });
    }

    public override void OnStop()
    {
        currentTowerBinding?.Dispose();
    }
}
