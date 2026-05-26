using System;
using System.Collections.Generic;
using System.Linq;
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Jomolith.Core;
using Jomolith.Core.Objects;
using Jomolith.Core.Objects.Enums;
using Jomolith.Utils.Towers.Objects;

namespace Jomolith.Utils.Towers;

public interface IWorkingTower : INode3D
{
    void Load(ITowerModel towerModel);
}

public partial class WorkingTower : Node3D, IWorkingTower
{
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
                PartType.Block => new Block(),
                PartType.Ball => new Ball(),
                PartType.Cylinder => new Cylinder(),
                PartType.Wedge => new Block(),
                PartType.CornerWedge => new Block(),
                _ => throw new ArgumentOutOfRangeException()
            };

            part.Initialize(partModel);

            AddChild(part);
        }
    }

    private void clearParts()
    {
        foreach (Node? child in GetChildren())
        {
            child.QueueFree();
            RemoveChild(child);
        }
    }
}
