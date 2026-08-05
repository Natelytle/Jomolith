using System;
using System.Collections.Generic;
using System.Numerics;
using Jomolith.Towers.Domain.Enums;
using Jomolith.Towers.Domain.Properties;

namespace Jomolith.Towers.Domain.Models;

public class PartModel
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; set; } = "Part";
    public PartType Shape { get; set; } = PartType.Block;

    public Vector3 Position { get; set; } = Vector3.Zero;
    public Quaternion Rotation { get; set; } = Quaternion.Identity;
    public Vector3 Scale { get; set; } = new(1, 2, 4);

    public float Height => Scale.X;
    public float CylinderRadius => Math.Min(Scale.Y, Scale.Z) / 2.0f;
    public float SphereRadius => Math.Min(Math.Min(Scale.X, Scale.Z), Scale.Y) / 2.0f;

    public bool CanCollide { get; set; } = true;
    public bool Anchored { get; set; } = true;

    public PhysicalProperties PhysicalProperties { get; set; } = new();
    public VisualProperties VisualProperties { get; set; } = new();

    public List<PartModel> Children { get; set; } = [];
}
