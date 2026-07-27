using System.Text.Json.Serialization;
using Jomolith.Settings.Models;

namespace Jomolith.Settings.Services;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
)]
[JsonSerializable(typeof(SettingsDto))]
public partial class SettingsJsonContext : JsonSerializerContext { }
