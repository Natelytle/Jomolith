using System;
using Jomolith.Tower.Core.Objects;

namespace Jomolith.Tower.State;

public partial class TowerLogic
{
    public static class Outputs
    {
        public readonly record struct SpawnPart(PartModel Model);

        public readonly record struct DespawnPart(Guid Id);
    }
}
