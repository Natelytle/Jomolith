using System.Collections.Generic;
using Jomolith.Towers.Models;

namespace Jomolith.Towers.Services;

public interface ITowerRepository
{
    IReadOnlyList<TowerDto> LoadAllTowers();
}
