using System.Text.Json;
using Jomolith.Towers.Models;
using Jomolith.Towers.Services;

namespace Jomolith.Tests.Towers;

public class TowerDtoDeserializationTests
{
    [Fact]
    public void Deserialize_ValidTowerJson_ParsesMetadataAndPartsCorrectly()
    {
        const string json = """
        {
          "metadata": {
            "name": "Tower of Blind Fate",
            "creator": "Nosav",
            "difficulty": 13,
            "version": 0
          },
          "parts": [
            {
              "type": "part",
              "shape": "block",
              "position": { "x": 215.55298, "y": 41.63095, "z": -358.2785 },
              "rotation": { "x": -0.00007341302, "y": 0.70710677, "z": 0.00007341302, "w": 0.70710677 },
              "scale": { "x": 7, "y": 1, "z": 4 },
              "can_collide": true,
              "anchored": false,
              "physical_properties": {
                "density": 1,
                "friction": 1,
                "elasticity": 0.5
              },
              "visual_properties": {
                "opacity": 1,
                "colour": "#00A378",
                "surface_type_xp": "studs",
                "surface_type_xn": "studs",
                "surface_type_yp": "studs",
                "surface_type_yn": "studs",
                "surface_type_zp": "studs",
                "surface_type_zn": "studs"
              },
              "name": "Part",
              "children": []
            }
          ]
        }
        """;

        // Act
        var tower = JsonSerializer.Deserialize(json, TowerJsonContext.Default.TowerDto);

        // Assert: Metadata
        Assert.NotNull(tower);
        Assert.Equal("Tower of Blind Fate", tower.Name);
        Assert.Equal("Nosav", tower.Creator);
        Assert.Equal(13, tower.Difficulty);

        // Assert: Parts
        Assert.Single(tower.Parts);
        var part = tower.Parts[0];

        Assert.Equal("Part", part.Name);
        Assert.Equal("part", part.Type);
        Assert.Equal("block", part.Shape);
        Assert.True(part.CanCollide);
        Assert.False(part.Anchored);

        // Assert: Vectors & Properties
        Assert.Equal(215.55298f, part.Position.X, precision: 4);
        Assert.Equal(0.70710677f, part.Rotation.Y, precision: 4);
        Assert.Equal(7f, part.Scale.X);

        Assert.Equal(0.5f, part.PhysicalProperties.Elasticity);
        Assert.Equal("#00A378", part.VisualProperties.ColourHex);
        Assert.Equal("studs", part.VisualProperties.SurfaceXp);
    }
}
