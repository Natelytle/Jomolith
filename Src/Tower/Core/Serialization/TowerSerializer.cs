using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using Godot;
using Jomolith.Tower.Core.Dto;
using Jomolith.Tower.Core.Objects;

namespace Jomolith.Tower.Core.Serialization;

public static class TowerSerializer
{
    public static TowerDto BuildTowerDto(TowerSceneModel scene, TowerMetadata metadata)
    {
        List<TowerObjectDto> rootChildren = scene.GetChildren(scene.RootId)
            .Select(id => buildObjectDto(scene, id))
            .ToList();

        return new TowerDto
        {
            Metadata = metadata,
            Objects = rootChildren
        };
    }

    private static TowerObjectDto buildObjectDto(TowerSceneModel scene, Guid objectId)
    {
        TowerObjectModel towerObjectModel = scene.FindPart(objectId);

        var children = scene.GetChildren(objectId).Select(id => buildObjectDto(scene, id)).ToList();

        TowerObjectDto dto = towerObjectModel switch
        {
            PartModel part => new PartDto
            {
                Name = part.Name,
                Shape = part.Shape,
                Position = part.Position,
                Rotation = part.Rotation,
                Scale = part.Scale,
                CanCollide = part.CanCollide,
                Anchored = part.Anchored,
                PhysicalProperties = part.PhysicalProperties,
                VisualProperties = part.VisualProperties,
                Children = children
            },
            _ => throw new InvalidOperationException($"Unknown object type {towerObjectModel.GetType().Name}")
        };

        return dto;
    }

    public static TowerModel CreateTowerFromDto(TowerDto dto)
    {
        TowerSceneModel scene = new TowerSceneModel();

        foreach (var rootPartDto in dto.Objects)
            addToSceneRecursive(scene, rootPartDto);

        return new TowerModel(scene, dto.Metadata);
    }

    private static void addToSceneRecursive(TowerSceneModel scene, TowerObjectDto towerObjectDto, Guid? parentId = null)
    {
        TowerObjectModel towerObjectModel = towerObjectDto switch
        {
            PartDto partDto => PartModel.FromDto(partDto),
            _ => throw new InvalidOperationException($"Unknown dto {towerObjectDto.GetType().Name}")
        };

        scene.AddTowerObject(towerObjectModel, parentId);

        foreach (var childDto in towerObjectDto.Children)
            addToSceneRecursive(scene, childDto, towerObjectModel.Id);
    }

    public static ITowerModel? FromJson(string json)
    {
        TowerDto? towerDto = JsonSerializer.Deserialize<TowerDto>(json);

        if (towerDto is null)
        {
            GD.PrintErr("Tower did not load successfully from JSON.");
            return null;
        }

        return CreateTowerFromDto(towerDto);
    }
}
