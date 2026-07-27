using Godot;
using Jomolith.Settings.Models;

namespace Jomolith.Settings.Services;

public interface ISettingsRepository
{
    SettingsDto Load();
    void Save(SettingsDto dto);
}
