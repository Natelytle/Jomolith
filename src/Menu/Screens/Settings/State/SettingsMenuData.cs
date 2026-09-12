using Jomolith.Settings;

namespace Jomolith.Menu.Screens.Settings.State;

public enum SettingsTab { Display, Gameplay }

public class SettingsMenuData
{
    public SettingsTab Tab { get; set; } = SettingsTab.Gameplay;
    public string? PendingRebindAction { get; set; }
    public UserSettings SettingsCache { get; set; } = null!;
}
