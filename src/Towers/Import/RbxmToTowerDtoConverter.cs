using System;
using System.Collections.Generic;
using System.IO;
using Jomolith.Towers.Import.Detectors;
using Jomolith.Towers.Import.Mappers;
using Jomolith.Towers.Models;
using RobloxFiles;

namespace Jomolith.Towers.Import;

public class RbxmToTowerDtoConverter
{
    private readonly List<IClientObjectDetector> detectors =
    [
        new SpinningPlatformDetector()
    ];

    public TowerDto ConvertFilePath(string rbxmPath)
    {
        RobloxFile file = RobloxFile.Open(rbxmPath);

        var clientObjects = new List<ClientObjectDto>();
        var processedInstances = new HashSet<Instance>();

        // Pass 1: Detect Interactive Client Objects (Spinners, Pushers, etc.)
        foreach (Instance descendant in file.GetDescendants())
        {
            if (processedInstances.Contains(descendant)) continue;

            foreach (var detector in detectors)
            {
                if (detector.TryExtract(descendant, out var clientObjectDto) && clientObjectDto != null)
                {
                    clientObjects.Add(clientObjectDto);
                    markProcessedRecursive(descendant, processedInstances);
                    break;
                }
            }
        }

        // Pass 2: Fallback Parts
        var parts = new List<PartDto>();
        foreach (Instance descendant in file.GetDescendants())
        {
            if (descendant is Part robloxPart && !processedInstances.Contains(robloxPart))
            {
                parts.Add(robloxPart.ToPartDto());
            }
        }

        string fileNameWithoutExt = Path.GetFileNameWithoutExtension(rbxmPath);

        // TODO: SpawnPosition is always 0
        var towerDto = new TowerDto(
            Name: fileNameWithoutExt,
            Creator: "Unknown",
            Difficulty: 0,
            Parts: parts,
            SpawnPosition: new Vector3Dto(0, 0, 0),
            ClientObjects: clientObjects
        );

        return towerDto;
    }

    private static void markProcessedRecursive(Instance instance, HashSet<Instance> processed)
    {
        processed.Add(instance);
        foreach (var child in instance.GetChildren())
        {
            markProcessedRecursive(child, processed);
        }
    }
}
