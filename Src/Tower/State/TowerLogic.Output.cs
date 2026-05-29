using System;
using Jomolith.Tower.Core.Objects;

namespace Jomolith.Tower.State;

public partial class TowerLogic
{
    public static class Output
    {
        public readonly record struct SpawnPart(PartModel Model);

        public readonly record struct DespawnPart(Guid Id);
    }
}
