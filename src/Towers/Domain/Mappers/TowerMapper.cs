using System;
using System.Numerics;
using Jomolith.Towers.Domain.Enums;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Domain.Properties;
using Jomolith.Towers.Models;

namespace Jomolith.Towers.Domain.Mappers;

public class TowerMapper
{
    public static TowerModel ToDomain(TowerDto dto)
    {
        var tower = new TowerModel
        {
            Name = dto.Metadata.Name,
            Creator = dto.Metadata.Creator,
            Difficulty = dto.Metadata.Difficulty,
            Version = dto.Metadata.Version
        };

        foreach (var partDto in dto.Parts)
        {
            tower.Parts.Add(ToDomain(partDto));
        }

        return tower;
    }

    public static PartModel ToDomain(PartDto dto)
    {
        var part = new PartModel
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Shape = parsePartType(dto.Shape),
            Position = new Vector3(dto.Position.X, dto.Position.Y, dto.Position.Z),
            Rotation = new Quaternion(dto.Rotation.X, dto.Rotation.Y, dto.Rotation.Z, dto.Rotation.W),
            Scale = new Vector3(dto.Scale.X, dto.Scale.Y, dto.Scale.Z),
            CanCollide = dto.CanCollide,
            Anchored = dto.Anchored,
            PhysicalProperties = new PhysicalProperties(
                dto.PhysicalProperties.Density,
                dto.PhysicalProperties.Friction,
                dto.PhysicalProperties.Elasticity
            ),
            VisualProperties = new VisualProperties(
                dto.VisualProperties.Opacity,
                dto.VisualProperties.ColourHex,
                parseSurfaceType(dto.VisualProperties.SurfaceXp),
                parseSurfaceType(dto.VisualProperties.SurfaceXn),
                parseSurfaceType(dto.VisualProperties.SurfaceYp),
                parseSurfaceType(dto.VisualProperties.SurfaceYn),
                parseSurfaceType(dto.VisualProperties.SurfaceZp),
                parseSurfaceType(dto.VisualProperties.SurfaceZn)
            )
        };

        foreach (var childDto in dto.Children)
        {
            part.Children.Add(ToDomain(childDto));
        }

        return part;
    }

    private static PartType parsePartType(string shape) => shape.ToLowerInvariant() switch
    {
        "block" => PartType.Block,
        "cylinder" => PartType.Cylinder,
        "ball" => PartType.Ball,
        "wedge" => PartType.Wedge,
        "corner_wedge" => PartType.CornerWedge,
        _ => PartType.Block
    };

    private static SurfaceType parseSurfaceType(string surface) => surface.ToLowerInvariant() switch
    {
        "smooth" => SurfaceType.Smooth,
        "glue" => SurfaceType.Glue,
        "weld" => SurfaceType.Weld,
        "studs" => SurfaceType.Studs,
        "inlet" => SurfaceType.Inlet,
        "universal" => SurfaceType.Universal,
        "hinge" => SurfaceType.Hinge,
        "motor" => SurfaceType.Motor,
        "stepping_motor" => SurfaceType.SteppingMotor,
        "smooth_no_outlines" => SurfaceType.SmoothNoOutlines,
        _ => SurfaceType.Smooth
    };
}
