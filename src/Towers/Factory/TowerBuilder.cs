
using System.Collections.Generic;
using Godot;
using Jomolith.Towers.Domain.Enums;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Factory.Shapes;

namespace Jomolith.Towers.Factory;

public class TowerBuilder
{
    // TODO: No support for wedges or corner wedges.
    private static readonly Dictionary<PartType, ShapeBuilder> shape_builders = new()
    {
        [PartType.Block] = new BlockShapeBuilder(),
        [PartType.Cylinder] = new CylinderShapeBuilder(),
        [PartType.Ball] = new BallShapeBuilder(),
        [PartType.Wedge] = new WedgeShapeBuilder(),
        [PartType.CornerWedge] = new CornerWedgeShapeBuilder()
    };

    private static readonly ShapeBuilder fallback_builder = new BlockShapeBuilder();

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
        CollisionObject3D partRoot;

        var shapeBuilder = shape_builders.GetValueOrDefault(part.Shape, fallback_builder);

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
                },
                Mass = part.PhysicalProperties.Density * shapeBuilder.GetVolume(part)
            };
        }

        partRoot.Name = part.Name;

        partRoot.Position = new Vector3(part.Position.X, part.Position.Y, part.Position.Z);
        partRoot.Quaternion = new Quaternion(part.Rotation.X, part.Rotation.Y, part.Rotation.Z, part.Rotation.W);

        if (part.VisualProperties.Opacity > 0.001f)
        {
            var meshInstance = new MeshInstance3D
            {
                Mesh = shapeBuilder.BuildMesh(part)
            };

            meshInstance.Scale = shapeBuilder.MeshScale(part);

            var color = Color.FromHtml(part.VisualProperties.ColourHex);
            color.A = part.VisualProperties.Opacity;

            meshInstance.MaterialOverride = new StandardMaterial3D
            {
                AlbedoColor = color,
                Transparency = part.VisualProperties.Opacity < 0.99f
                    ? BaseMaterial3D.TransparencyEnum.Alpha
                    : BaseMaterial3D.TransparencyEnum.Disabled
            };

            meshInstance.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;

            partRoot.AddChild(meshInstance);
        }

        // We don't generate a collision shape when previewing the tower
        if (part.CanCollide && !isPreview)
        {
            var collisionShape = new CollisionShape3D
            {
                Shape = shapeBuilder.BuildCollisionShape(part)
            };

            partRoot.AddChild(collisionShape);

            // Cylinders are centered around the X axis in Roblox, but around the Y axis in Godot
            // We rotate cylinder hitboxes around Z by 90 degrees to account for this.
            if (part.Shape is PartType.Cylinder)
                collisionShape.RotateObjectLocal(new Vector3(0, 0, 1), float.Pi / 2.0f);
            else if (part.Shape is PartType.Wedge)
                collisionShape.RotateObjectLocal(new Vector3(0, 1, 0), float.Pi / 2.0f);

            // Add camera collision
            if (part.VisualProperties.Opacity >= 0.99f)
            {
                const int camera_collision_layer = 2;

                partRoot.CollisionLayer |= camera_collision_layer;
            }
        }

        foreach (var childPart in part.Children)
        {
            partRoot.AddChild(buildPart(childPart, isPreview));
        }

        return partRoot;
    }
}
