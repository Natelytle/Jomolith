using System.Collections.Generic;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Models;

namespace Jomolith.Towers.Services;

public interface ITowerRepository
{
    IReadOnlyList<TowerModel> LoadAllTowers();
}
