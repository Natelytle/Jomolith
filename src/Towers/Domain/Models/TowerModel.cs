using System.Collections.Generic;

namespace Jomolith.Towers.Domain.Models;

public class TowerModel
{
    public string Name { get; set; } = string.Empty;
    public string Creator { get; set; } = string.Empty;
    public double Difficulty { get; set; }

    public List<PartModel> Parts { get; set; } = [];
}
