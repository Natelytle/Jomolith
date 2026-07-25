using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record ClientObjectDto(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("client_object_type")] string Type,
    [property: JsonPropertyName("kit_version")] string KitVersion,
    [property: JsonPropertyName("properties")] IReadOnlyDictionary<string, object> Properties,
    [property: JsonPropertyName("parts")] IReadOnlyList<PartDto> Parts
);
