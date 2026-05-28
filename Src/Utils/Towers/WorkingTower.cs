using System;
using System.Collections.Generic;
using System.Linq;
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Jomolith.Core;
using Jomolith.Core.Objects;
using Jomolith.Core.Objects.Enums;
using Jomolith.Utils.Rendering.Parts;
using Jomolith.Utils.Towers.Objects;

namespace Jomolith.Utils.Towers;

public interface IWorkingTower : INode3D
{
    void Initialize();
    void Load(ITowerModel towerModel);
}

public partial class WorkingTower : Node3D, IWorkingTower
{
    [Export] public Mesh[] Meshes;
    [Export] public Material[] Materials;

    private readonly List<Part> parts = new List<Part>();
    private PartHeap heap;

    public void Initialize()
    {
        heap = new PartHeap
        {
            Meshes = Meshes,
            Materials = Materials,

            UsesColor = true,
            UsesCustomData = true,

            BlockCapacity = 2048,
            BlockGeometricSorting = false,
            BlockGeometricSize = 1024,

            DefragThreshold = 0.5f,
            MaxSwapsPerFrame = 1024
        };

        AddChild(heap);
    }

    public void Load(ITowerModel towerModel)
    {
        clearParts();

        foreach (TowerObjectModel towerObject in towerModel.TowerSceneModel.GetDescendants(towerModel.TowerSceneModel.RootId).Select(towerModel.TowerSceneModel.FindPart))
        {
            addPart(towerObject);
        }
    }

    private void addPart(TowerObjectModel towerObjectModel)
    {
        if (towerObjectModel is PartModel partModel)
        {
            Part part = partModel.Shape switch
            {
                PartType.Block => new BlockPart(),
                PartType.Ball => new BallPart(),
                PartType.Cylinder => new CylinderPart(),
                PartType.Wedge => new BlockPart(),
                PartType.CornerWedge => new BlockPart(),
                _ => throw new ArgumentOutOfRangeException()
            };

            part.Initialize(partModel);
            part.AddToPartHeap(heap);
            parts.Add(part);

            AddChild(part);
        }
    }

    private void clearParts()
    {
        foreach (Part part in parts)
        {
            part.RemoveFromPartHeap();
            part.QueueFree();
            RemoveChild(part);
        }
    }
}
