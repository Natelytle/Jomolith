using System.Collections.Generic;
using RobloxFiles.DataTypes;

namespace Jomolith.Towers.Import.Mappers;

using System.Numerics;
using RobloxFiles;
using RobloxFiles.Enums;
using Models;
using Domain.Enums;

public static class RobloxPartExtensions
{
    public static PartDto ToPartDto(this FormFactorPart robloxPart)
    {
        float[] c = robloxPart.CFrame.Rotation.GetComponents();
        var rotationMatrix = new Matrix4x4(
            c[3], c[6], c[9],  0,
            c[4], c[7], c[10], 0,
            c[5], c[8], c[11], 0,
            0,    0,    0,     1
        );
        var q = Quaternion.CreateFromRotationMatrix(rotationMatrix);

        Domain.Enums.PartType shape = Domain.Enums.PartType.Block;

        if (robloxPart is Part part)
        {
            shape = part.Shape switch
            {
                RobloxFiles.Enums.PartType.Block => Domain.Enums.PartType.Block,
                RobloxFiles.Enums.PartType.Ball => Domain.Enums.PartType.Ball,
                RobloxFiles.Enums.PartType.Cylinder => Domain.Enums.PartType.Cylinder,
                _ => shape // Fallback
            };
        }
        else if (robloxPart is WedgePart)
        {
            shape = Domain.Enums.PartType.Wedge;
        }

        return new PartDto
        (
            Name: robloxPart.Name,
            Shape: mapPartType(shape),
            Position: new Vector3Dto(robloxPart.CFrame.Position.X, robloxPart.CFrame.Position.Y, robloxPart.CFrame.Position.Z),
            Rotation: new QuaternionDto(q.X, q.Y, q.Z, q.W),
            Scale: new Vector3Dto(robloxPart.Size.X, robloxPart.Size.Y, robloxPart.Size.Z),
            Anchored: robloxPart.Anchored,
            CanCollide: robloxPart.CanCollide,
            PhysicalProperties: new PhysicalPropertiesDto
            (
                Friction: robloxPart.CustomPhysicalProperties?.Friction ?? 1.0f,
                Density: robloxPart.CustomPhysicalProperties?.Density ?? 1.0f,
                Elasticity: robloxPart.CustomPhysicalProperties?.Elasticity ?? 0.5f
            ),
            VisualProperties: new VisualPropertiesDto
            (
                Opacity: 1.0f - robloxPart.Transparency,
                ColourHex: robloxColor3ToHex(robloxPart.Color),
                SurfaceZn: mapSurfaceType(robloxPart.FrontSurface),
                SurfaceZp: mapSurfaceType(robloxPart.BackSurface),
                SurfaceXn: mapSurfaceType(robloxPart.LeftSurface),
                SurfaceXp: mapSurfaceType(robloxPart.RightSurface),
                SurfaceYn: mapSurfaceType(robloxPart.BottomSurface),
                SurfaceYp: mapSurfaceType(robloxPart.TopSurface)
            ),
            Type: "part",
            Children: []
        );
    }

    private static string robloxColor3ToHex(Color3 color)
    {
        int r = (int)(color.R * 255);
        int g = (int)(color.G * 255);
        int b = (int)(color.B * 255);

        return $"#{r:X2}{g:X2}{b:X2}";
    }

    private static string mapPartType(Domain.Enums.PartType shape) => shape switch
    {
        Domain.Enums.PartType.Block => "block",
        Domain.Enums.PartType.Cylinder => "cylinder",
        Domain.Enums.PartType.Ball => "ball",
        Domain.Enums.PartType.Wedge => "wedge",
        Domain.Enums.PartType.CornerWedge => "corner_wedge",
        _ => "block"
    };

    private static string mapSurfaceType(RobloxFiles.Enums.SurfaceType surfaceType) => surfaceType switch
    {
        RobloxFiles.Enums.SurfaceType.Smooth => "smooth",
        RobloxFiles.Enums.SurfaceType.Glue => "glue",
        RobloxFiles.Enums.SurfaceType.Weld => "weld",
        RobloxFiles.Enums.SurfaceType.Studs => "studs",
        RobloxFiles.Enums.SurfaceType.Inlet => "inlet",
        RobloxFiles.Enums.SurfaceType.Universal => "universal",
        RobloxFiles.Enums.SurfaceType.Hinge => "hinge",
        RobloxFiles.Enums.SurfaceType.Motor => "motor",
        RobloxFiles.Enums.SurfaceType.SteppingMotor => "stepping_motor",
        RobloxFiles.Enums.SurfaceType.SmoothNoOutlines => "smooth_no_outlines",
        _ => "smooth"
    };
}
