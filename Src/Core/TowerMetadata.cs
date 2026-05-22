using System.Text.Json.Serialization;

namespace Jomolith.Core;

public struct TowerMetadata
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("creator")] public string Creator { get; set; }

    [JsonPropertyName("difficulty")] public double Difficulty { get; set; }
}
