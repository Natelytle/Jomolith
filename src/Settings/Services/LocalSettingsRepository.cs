using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using Jomolith.Settings.Models;
using FileAccess = Godot.FileAccess;

namespace Jomolith.Settings.Services;

public class LocalSettingsRepository : ISettingsRepository
{
    private static readonly string[] rebindable_actions = ["move_forward", "move_back", "move_left", "move_right", "jump", "shift_lock"];

    private readonly string targetPath;

    public LocalSettingsRepository(string relativePath = "user://settings.json")
    {
        targetPath = ProjectSettings.GlobalizePath(relativePath);
    }

    public SettingsDto Load()
    {
        var settings = defaultSettings();

        // If the settings file does not exist, we create it and return the default.
        if (!FileAccess.FileExists(targetPath))
        {
            Save(settings);

            return settings;
        }

        try
        {
            string json = File.ReadAllText(targetPath);
            var settingsDto = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.SettingsDto);

            if (settingsDto is not null)
                settings = settingsDto;

        }
        catch (System.Exception ex)
        {
            GD.PrintErr($"Failed to parse settings at {targetPath}: {ex.Message}");
        }

        return settings;
    }

    public void Save(SettingsDto dto)
    {
        File.WriteAllText(targetPath, JsonSerializer.Serialize(dto, SettingsJsonContext.Default.SettingsDto));
    }

    private static SettingsDto defaultSettings() => new
    (
        CameraSensitivity: 1f,
        KeyBindings: rebindable_actions.ToDictionary(
            action => action,
            action => (InputMap.ActionGetEvents(action).OfType<InputEventKey>().FirstOrDefault()?.PhysicalKeycode ?? Key.None).ToString()
        )
    );
}
