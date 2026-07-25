using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record TowerDto(
    [property: JsonPropertyName("metadata")] TowerMetadataDto Metadata,
    [property: JsonPropertyName("parts")] List<PartDto> Parts,
    [property: JsonPropertyName("client_objects")] List<ClientObjectDto> ClientObjects
);
