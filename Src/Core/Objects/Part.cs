using System;
using System.Numerics;
using System.Text.Json.Serialization;
using Jomolith.Core.Objects.Enums;
using Jomolith.Core.Objects.Properties;
using Jomolith.Core.SerializationUtils;

namespace Jomolith.Core.Objects;

/// <summary>
///     The part class, which contains per-part information.
/// </summary>
public class Part : TowerObject
{
    public Part()
    {
        Shape = PartType.Block;
        Position = Vector3.Zero;
        Rotation = Quaternion.Identity;
        Scale = new Vector3(2, 2, 4);
        CanCollide = true;
        Anchored = true;
        PhysicalProperties = new PhysicalProperties();
        VisualProperties = new VisualProperties();
    }

    public PartType Shape { get; set; }

    public SerializableVector3 Position { get; set; }

    public SerializableQuaternion Rotation { get; set; }

    public SerializableVector3 Scale { get; set; }

    public float Height => Scale.Y;
    public float CylinderRadius => Math.Min(Scale.X, Scale.Z);
    public float SphereRadius => Math.Min(CylinderRadius, Scale.Y);

    public bool CanCollide { get; set; }

    public bool Anchored { get; set; }

    public PhysicalProperties PhysicalProperties { get; set; }

    public VisualProperties VisualProperties { get; set; }

    public static Part FromDto(PartDto dto)
    {
        // We don't copy over children, we let addPartRecursive handle this in TowerData.
        Part part = new Part
        {
            Name = dto.Name,
            Shape = dto.Shape,
            Position = dto.Position,
            Rotation = dto.Rotation,
            Scale = dto.Scale,
            CanCollide = dto.CanCollide,
            Anchored = dto.Anchored,
            PhysicalProperties = dto.PhysicalProperties,
            VisualProperties = dto.VisualProperties,
            Id = Guid.NewGuid()
        };

        return part;
    }
}
