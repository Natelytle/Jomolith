using System.Collections.Generic;
using System.Text.Json.Serialization;
using Jomolith.Core.Objects;

namespace Jomolith.Core;

public class TowerDto
{
    [JsonPropertyName("metadata")] public required TowerMetadata Metadata { get; set; }

    [JsonPropertyName("parts")] public required List<TowerObjectDto> Objects { get; set; }
}
