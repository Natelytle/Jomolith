namespace Jomolith.Tower.State;

using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

public partial class TowerLogic
{
    [Meta]
    public abstract partial record TowerState : StateLogic<TowerState>;
}
