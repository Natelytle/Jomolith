using System.Collections.Generic;
using System.Text.Json;
using Godot;
using Jomolith.Towers.Import.Mappers;
using RobloxFiles;
using Jomolith.Towers.Models;
using Jomolith.Towers.Services;

namespace Jomolith.Towers.Import.Detectors;

public class MovingPlatformDetector : IClientObjectDetector
{
    public bool TryExtract(Instance instance, out ClientObjectDto? dto)
    {
        dto = null;

        // Default delay value for a moving platform is 5.
        // TODO: Shoving platforms can have specific delays/tweens for each movement.
        double delaySecs = 5;
        List<Vector3Dto>? positions;
        PartDto? platform = null;

        var children = instance.Children;

        if (instance.Name is "Moving Platform" or "MovingPlatform")
        {
            positions = new List<Vector3Dto>();

            foreach (var child in children)
            {
                switch (child)
                {
                    case NumberValue numberValue when numberValue.Name == "Delay":
                    {
                        delaySecs = numberValue.Value;
                        break;
                    }
                    case Model model when model.Name == "Positions":
                    {
                        foreach (var positionChild in model.Children)
                        {
                            if (positionChild is Part position)
                                positions.Add(new Vector3Dto(
                                    position.CFrame.Position.X,
                                    position.CFrame.Position.Y,
                                    position.CFrame.Position.Z)
                                );
                        }

                        break;
                    }
                    case Part part when part.Name == "Platform":
                    {
                        platform = part.ToPartDto();
                        break;
                    }
                }
            }

            if (positions.Count == 0 || platform is null)
            {
                GD.Print($"Moving platform missing required children: {instance.Name} @{instance.UniqueId}");
                return false;
            }

            dto = new ClientObjectDto(
                Name: instance.Name,
                Type: "moving_platform",
                Properties: new Dictionary<string, JsonElement>
                {
                    ["delay"] = JsonSerializer.SerializeToElement(delaySecs, TowerJsonContext.Default.Double),
                    ["positions"] = JsonSerializer.SerializeToElement(positions, TowerJsonContext.Default.ListVector3Dto)
                },
                Parts: [platform]
            );

        }

        return dto is not null;
    }
}
