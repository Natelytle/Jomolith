using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
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

    public TowerDto ConvertRobloxFile(RobloxFile file, string fileName)
    {
        var clientObjects = new List<ClientObjectDto>();
        var processedInstances = new HashSet<Instance>();

        // 1: Client Objects
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

        // 2: Parts
        var parts = new List<PartDto>();
        foreach (Instance descendant in file.GetDescendants())
        {
            if (descendant is FormFactorPart robloxPart && !processedInstances.Contains(robloxPart))
            {
                parts.Add(robloxPart.ToPartDto());
            }
        }

        // 3: Spawn Location
        Vector3Dto spawnPosition = new Vector3Dto(0, 0, 0);

        var spawn = (SpawnLocation?)file.GetDescendants().FirstOrDefault(p => p is SpawnLocation);

        if (spawn != null)
        {
            spawnPosition = new Vector3Dto(spawn.CFrame.Position.X, spawn.CFrame.Position.Y, spawn.CFrame.Position.Z);
        }

        // TODO: SpawnPosition is always 0
        var towerDto = new TowerDto(
            Name: fileName,
            Creator: "Unknown",
            Difficulty: 0,
            Parts: parts,
            SpawnPosition: spawnPosition,
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
