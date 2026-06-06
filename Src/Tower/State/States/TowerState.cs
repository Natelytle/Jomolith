using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Tower.State.States;

[Meta, StateDiagram]
public abstract partial record TowerState : LogicBlockState;
