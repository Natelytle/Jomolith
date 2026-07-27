using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Settings.Models;

public record SettingsDto(
    [property: JsonPropertyName("camera_sensitivity")] float CameraSensitivity,
    [property: JsonPropertyName("key_bindings")] Dictionary<string, string> KeyBindings
);
