using System.Collections.Generic;
using Godot;

namespace Jomolith.Menu.Screens.Settings.State;

public enum SettingsTab { Display, Gameplay }

public class SettingsData
{
    public SettingsTab Tab { get; set; } = SettingsTab.Gameplay;
    public float CameraSensitivity { get; set; } = 1f;
    public Dictionary<string, Key> KeyBindings { get; set; } = new();
    public string? PendingRebindAction { get; set; }
}
