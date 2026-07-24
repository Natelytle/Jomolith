using System.Collections.Generic;
using Godot;
using Jomolith.Towers.Domain.Enums;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Factory.Shapes;

namespace Jomolith.Towers.Factory;

public class TowerBuilder
{
    // TODO: No support for wedges or corner wedges.
    private static readonly Dictionary<PartType, IShapeBuilder> shape_builders = new()
    {
        [PartType.Block] = new BlockShapeBuilder(),
        [PartType.Cylinder] = new CylinderShapeBuilder(),
        [PartType.Ball] = new BallShapeBuilder(),
        [PartType.Wedge] = new WedgeShapeBuilder(),
        [PartType.CornerWedge] = new CornerWedgeShapeBuilder()
    };

    private static readonly IShapeBuilder fallback_builder = new BlockShapeBuilder();

    /// <summary>
    /// Builds the actual node tree that contains everything represented in a Tower DTO.
    /// </summary>
    /// <param name="tower">The DTO representation of the tower to build.</param>
    /// <param name="isPreview">Whether or not the tower should be visual only, aka no collision shapes or unanchored parts.</param>
    /// <returns>A Node3D with everything in the DTO.</returns>
    public Node3D BuildTower(TowerModel tower, bool isPreview = false)
    {
        var root = new Node3D { Name = tower.Name };

        foreach (var part in tower.Parts)
        {
            root.AddChild(buildPart(part, isPreview));
        }

        return root;
    }

    /// <summary>
    /// Recursively builds a single part (and children) as a godot node tree.
    /// </summary>
    /// <param name="part">The part model to represent.</param>
    /// <param name="isPreview">If true, the tree will not generate collision shapes.</param>
    /// <returns>The node tree.</returns>
    private Node3D buildPart(PartModel part, bool isPreview)
    {
        Node3D partRoot;

        if (isPreview || part.Anchored)
        {
            partRoot = new StaticBody3D();
        }
        else
        {
            partRoot = new RigidBody3D
            {
                PhysicsMaterialOverride = new PhysicsMaterial
                {
                    Friction = part.PhysicalProperties.Friction,
                    Bounce = part.PhysicalProperties.Elasticity
                }
            };
        }

        partRoot.Name = part.Name;

        partRoot.Position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
        partRoot.Quaternion = new Quaternion(part.Rotation.X, part.Rotation.Y, part.Rotation.Z, part.Rotation.W);
        partRoot.Scale = new Vector3(part.Scale.X, part.Scale.Y, part.Scale.Z);

        var shapeBuilder = shape_builders.GetValueOrDefault(part.Shape, fallback_builder);

        if (part.VisualProperties.Opacity > 0.001f)
        {
            var meshInstance = new MeshInstance3D
            {
                Mesh = shapeBuilder.BuildMesh(part)
            };

            var color = Color.FromHtml(part.VisualProperties.ColourHex);
            color.A = part.VisualProperties.Opacity;

            meshInstance.MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = color,
                Transparency = part.VisualProperties.Opacity < 0.99f
                    ? BaseMaterial3D.TransparencyEnum.Alpha
                    : BaseMaterial3D.TransparencyEnum.Disabled
            };

            partRoot.AddChild(meshInstance);
        }

        // We don't generate a collision shape when previewing the tower
        if (part.CanCollide && !isPreview)
        {
            partRoot.AddChild(new CollisionShape3D
            {
                Shape = shapeBuilder.BuildCollisionShape(part)
            });
        }

        foreach (var childPart in part.Children)
        {
            partRoot.AddChild(buildPart(childPart, isPreview));
        }

        return partRoot;
    }
}
