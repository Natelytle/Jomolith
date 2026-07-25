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

        if (instance.Name is "Moving Platform" or "MovingPlatform")
        {
            // Default delay value for a moving platform is 5.
            // TODO: Shoving platforms can have specific delays/tweens for each movement.
            double delaySecs = 5;

            var positions = new List<Vector3Dto>();
            var platformParts = new List<PartDto>();

            var children = instance.Children;

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
                    case Part part:
                    {
                        platformParts.Add(part.ToPartDto());
                        break;
                    }
                }
            }

            if (positions.Count == 0 || platformParts.Count == 0)
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
                Parts: new Dictionary<string, List<PartDto>> {
                    ["platform"] = platformParts
                }
            );

        }

        return dto is not null;
    }
}
