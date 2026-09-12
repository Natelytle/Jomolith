using System.Text.Json.Serialization;

namespace Jomolith.Settings;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
)]
[JsonSerializable(typeof(SettingsDto))]
public partial class SettingsJsonContext : JsonSerializerContext { }
