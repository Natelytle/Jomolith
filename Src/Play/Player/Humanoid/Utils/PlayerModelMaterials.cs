using System.Collections.Generic;
using Godot;

namespace Jomolith.Play.Player.Humanoid.Utils;

public class PlayerModelMaterials
{
    private readonly StandardMaterial3D[] materials;

    public PlayerModelMaterials(IPlayerModel model)
    {
        materials = duplicateMaterials(model);
    }

    public void SetOpacity(float alpha)
    {
        foreach (StandardMaterial3D material in materials)
            material.SetAlbedo(material.AlbedoColor with { A = alpha });
    }

    private StandardMaterial3D[] duplicateMaterials(IPlayerModel model)
    {
        List<StandardMaterial3D> materialsList = new List<StandardMaterial3D>();

        foreach (Node node in model.FindChildren("*", "MeshInstance3D", true, false))
        {
            MeshInstance3D mesh = (MeshInstance3D)node;

            for (int i = 0; i < mesh.GetSurfaceOverrideMaterialCount(); i++)
            {
                Material material = mesh.GetSurfaceOverrideMaterial(i);

                if (material is null)
                    material = mesh.Mesh.SurfaceGetMaterial(i);

                if (material is StandardMaterial3D standardMaterial)
                {
                    StandardMaterial3D duped = (StandardMaterial3D)standardMaterial.Duplicate();
                    duped.Transparency = BaseMaterial3D.TransparencyEnum.Alpha;
                    duped.DepthDrawMode = BaseMaterial3D.DepthDrawModeEnum.Always;

                    // TODO: Fix this hack
                    if (mesh.Name == "Head_2")
                        duped.RenderPriority = 0;
                    else if (mesh.Name == "Hair")
                        duped.RenderPriority = 1;
                    else if (mesh.Name == "Face")
                        duped.RenderPriority = 2;

                    mesh.SetSurfaceOverrideMaterial(i, duped);
                    materialsList.Add(duped);
                }
            }
        }

        return materialsList.ToArray();
    }
}
