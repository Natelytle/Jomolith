using System.Collections.Generic;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Menu.Screens.Select.State;

public class TowerSelectionData
{
    public IReadOnlyList<TowerModel> Towers { get; set; } = new List<TowerModel>();
    public int SelectedIndex { get; set; } = -1;
}
