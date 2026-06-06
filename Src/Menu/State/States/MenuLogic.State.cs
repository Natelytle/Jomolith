using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;

namespace Jomolith.Menu.State.States;

[Meta, StateDiagram]
public abstract partial record MenuState : LogicBlockState;
