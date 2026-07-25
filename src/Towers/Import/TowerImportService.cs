using System;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using Jomolith.Towers.Models;
using Jomolith.Towers.Services;

namespace Jomolith.Towers.Import;

public partial class TowerImportService : Node
{
    private const string tower_directory = "user://towers/";

    public override void _Ready()
    {
        DirAccess.MakeDirAbsolute(tower_directory);

        GetWindow().FilesDropped += onFilesDropped;
    }

    public override void _ExitTree()
    {
        GetWindow().FilesDropped -= onFilesDropped;
    }

    private async void onFilesDropped(string[] files)
    {
        foreach (string filePath in files)
        {
            string extension = Path.GetExtension(filePath).ToLower();

            if (extension == ".rbxm" || extension == ".rbxl")
            {
                GD.Print($"[TowerImport] Detected Roblox model: {filePath}");
                await processRobloxFileAsync(filePath);
            }
        }
    }

    private async Task processRobloxFileAsync(string filePath)
    {
        try
        {
            TowerDto towerDtoOutput = await Task.Run(() =>
            {
                var converter = new RbxmToTowerDtoConverter();
                return converter.ConvertFilePath(filePath);
            });

            string targetPath = ProjectSettings.GlobalizePath($"{tower_directory}{towerDtoOutput.Metadata.Name}.json");

            string json = JsonSerializer.Serialize(towerDtoOutput, TowerJsonContext.Default.TowerDto);

            await File.WriteAllTextAsync(targetPath, json);

            GD.Print($"[TowerImport] Successfully imported tower to: {targetPath}");

            // TODO: Notify ILocalTowerRepository to reload the list
        }
        catch (Exception ex)
        {
            GD.PrintErr($"[TowerImport] Failed to convert '{filePath}': {ex.Message}\n{ex.StackTrace}");
        }
    }
}
