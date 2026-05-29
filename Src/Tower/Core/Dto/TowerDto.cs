using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Tower.Core.Dto;

public class TowerDto
{
    [JsonPropertyName("metadata")] public required TowerMetadata Metadata { get; set; }

    [JsonPropertyName("parts")] public required List<TowerObjectDto> Objects { get; set; }
}
