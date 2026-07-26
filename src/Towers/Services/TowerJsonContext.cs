using System.Text.Json.Serialization;
using Jomolith.Towers.Models;

namespace Jomolith.Towers.Services;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
)]
[JsonSerializable(typeof(TowerDto))]
public partial class TowerJsonContext : JsonSerializerContext
{
}
