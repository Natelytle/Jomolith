using System.Collections.Generic;
using System.Text.Json.Serialization;
using Jomolith.Towers.Models;

namespace Jomolith.Towers.Services;

[JsonSourceGenerationOptions(
    PropertyNameCaseInsensitive = true,
    WriteIndented = true
)]
[JsonSerializable(typeof(TowerDto))]
[JsonSerializable(typeof(double))]
[JsonSerializable(typeof(float))]
[JsonSerializable(typeof(int))]
[JsonSerializable(typeof(string))]
[JsonSerializable(typeof(bool))]
[JsonSerializable(typeof(List<Vector3Dto>))]
public partial class TowerJsonContext : JsonSerializerContext
{
}
