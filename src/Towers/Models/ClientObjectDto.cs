using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Jomolith.Towers.Models;

public record ClientObjectDto(
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("client_object_type")] string Type,
    [property: JsonPropertyName("properties")] IReadOnlyDictionary<string, JsonElement> Properties,
    [property: JsonPropertyName("parts")] IReadOnlyList<PartDto> Parts
);
