using System.Collections.Generic;
using System.Numerics;

namespace Jomolith.Towers.Domain.Models;

public class TowerModel
{
    public string Name { get; set; } = string.Empty;
    public string Creator { get; set; } = string.Empty;
    public double Difficulty { get; set; }
    public Vector3 SpawnPosition { get; set; } = Vector3.Zero;

    public List<PartModel> Parts { get; set; } = [];
}
