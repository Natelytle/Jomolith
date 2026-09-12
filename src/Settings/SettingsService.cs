using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;

namespace Jomolith.Settings;

public interface ISettingsService
{
    UserSettings Load();
    void Save(UserSettings dto);
    void ApplyBindings(KeyBindings keyBindings);
    void UpdateBinding(string action, Key key);
}

public class SettingsService : ISettingsService
{
    private readonly string targetPath;

    public SettingsService(string relativePath = "user://settings.json")
    {
        targetPath = ProjectSettings.GlobalizePath(relativePath);
    }

    public UserSettings Load()
    {
        var settings = defaultSettings();

        // If the settings file does not exist, we create it and return the default.
        if (!Godot.FileAccess.FileExists(targetPath))
        {
            Save(settings);

            return settings;
        }

        try
        {
            string json = File.ReadAllText(targetPath);
            var settingsDto = JsonSerializer.Deserialize(json, SettingsJsonContext.Default.SettingsDto);

            if (settingsDto is not null)
            {
                settings.CameraSensitivity = settingsDto.CameraSensitivity;

                // Get the number of key bindings
                int totalKeyBindings = settings.KeyBindings.Count;
                int loadedKeyBindings = 0;

                foreach (var (key, value) in settingsDto.KeyBindings)
                {
                    if (!settings.KeyBindings.ContainsKey(key))
                        continue;

                    settings.KeyBindings[key] = Enum.Parse<Key>(value);
                    loadedKeyBindings++;
                }

                // Save the settings to update the file with all key bindings, if we are missing some
                if (totalKeyBindings - loadedKeyBindings > 0)
                    Save(settings);
            }
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Failed to parse settings at {targetPath}: {ex.Message}");
        }

        return settings;
    }

    public void Save(UserSettings settings)
    {
        var dto = new SettingsDto(
            settings.CameraSensitivity,
            settings.KeyBindings.ToDictionary(
            binding => binding.Key,
            binding => binding.Value.ToString())
        );

        File.WriteAllText(targetPath, JsonSerializer.Serialize(dto, SettingsJsonContext.Default.SettingsDto));
    }

    public void ApplyBindings(KeyBindings keyBindings)
    {
        foreach ((string action, Key key) in keyBindings)
        {
            UpdateBinding(action, key);
        }
    }

    public void UpdateBinding(string action, Key key)
    {
        InputMap.ActionEraseEvents(action);
        InputMap.ActionAddEvent(action, new InputEventKey { PhysicalKeycode = key });
    }

    private static UserSettings defaultSettings() => new()
    {
        CameraSensitivity = 1f,
        KeyBindings = KeyBindings.DEFAULT
    };
}
