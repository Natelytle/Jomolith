using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using Jomolith.Towers.Domain.Mappers;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Towers.Services;

public class LocalTowerRepository : ITowerRepository
{
    private static readonly JsonSerializerOptions json_options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly string targetPath;

    public LocalTowerRepository(string relativePath = "user://towers")
    {
        targetPath = ProjectSettings.GlobalizePath(relativePath);
    }

    public IReadOnlyList<TowerModel> LoadAllTowers()
    {
        var towers = new List<TowerModel>();

        if (!Directory.Exists(targetPath))
        {
            Directory.CreateDirectory(targetPath);
            return towers;
        }

        foreach (string filePath in Directory.GetFiles(targetPath, "*.json"))
        {
            try
            {
                string json = File.ReadAllText(filePath);
                var towerDto = JsonSerializer.Deserialize(json, TowerJsonContext.Default.TowerDto);

                if (towerDto != null)
                    towers.Add(TowerMapper.ToDomain(towerDto));
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"Failed to parse tower at {filePath}: {ex.Message}");
            }
        }

        return towers;
    }
}
