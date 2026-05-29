using System;
using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Tower.Core.Objects.Enums;
using Jomolith.Tower.Domain;
using Jomolith.Tower.State;
using Jomolith.Utils.Towers.Objects;

namespace Jomolith.Tower;

public interface ITower : INode3D
{
}

[Meta(typeof(IAutoNode))]
public partial class Tower : Node3D, ITower
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    // The tower is lower in the hierarchy than what needs the tower repo (Gameplay/Editor),
    // so it is treated as a dependency from gameplay/editor instead of us providing it.
    [Dependency] public ITowerRepo TowerRepo => this.DependOn<ITowerRepo>();

    #endregion

    #region State

    public ITowerLogic TowerLogic { get; set; } = null!;

    public TowerLogic.IBinding TowerBinding { get; set; } = null!;

    private readonly Dictionary<Guid, Part> spawnedParts = new Dictionary<Guid, Part>();

    #endregion

    public void Setup()
    {
        TowerLogic = new TowerLogic();

        TowerLogic.Set(TowerRepo);
    }

    public void OnResolved()
    {
        TowerBinding = TowerLogic.Bind();

        TowerBinding
            .Handle((in TowerLogic.Output.SpawnPart output) =>
            {
                Part part = output.Model.Shape switch
                {
                    PartType.Block => new BlockPart(),
                    PartType.Ball => new BallPart(),
                    PartType.Cylinder => new CylinderPart(),
                    PartType.Wedge => new BlockPart(),
                    PartType.CornerWedge => new BlockPart(),
                    _ => throw new ArgumentOutOfRangeException()
                };

                part.Initialize(output.Model);
                spawnedParts[output.Model.Id] = part;
                AddChild(part);
            })
            .Handle((in TowerLogic.Output.DespawnPart output) =>
            {
                if (spawnedParts.TryGetValue(output.Id, out var part))
                {
                    part.QueueFree();
                    spawnedParts.Remove(output.Id);
                }
            });

        TowerLogic.Start();
    }
}
