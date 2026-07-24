using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using Jomolith.Towers.Models;

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

    public IReadOnlyList<TowerDto> LoadAllTowers()
    {
        var towers = new List<TowerDto>();

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
                var tower = JsonSerializer.Deserialize(json, TowerJsonContext.Default.TowerDto);

                if (tower != null)
                    towers.Add(tower);
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"Failed to parse tower at {filePath}: {ex.Message}");
            }
        }

        return towers;
    }
}
