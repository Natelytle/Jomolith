using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.App.State.States;

[Meta, StateDiagram]
public abstract partial record AppState : LogicBlockState;
