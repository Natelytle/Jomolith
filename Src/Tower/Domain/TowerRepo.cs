
using System;
using Chickensoft.Sync.Primitives;
using Jomolith.Tower.Core;
using Jomolith.Tower.Core.Objects;

namespace Jomolith.Tower.Domain;

public interface ITowerRepo
{
    IAutoChannel AutoChannel { get; }
    IAutoValue<ITowerModel?> CurrentTower { get; }

    readonly record struct ObjectAdded(TowerObjectModel Model);
    readonly record struct ObjectRemoved(Guid Id);
    readonly record struct ObjectChanged(TowerObjectModel Model);
    readonly record struct ObjectReparented(Guid Id, Guid NewParent);
}

public interface IEditableTowerRepo : ITowerRepo
{
    void LoadTower(ITowerModel towerModel);
    void UnloadTower();

    void AddObject(TowerObjectModel model, Guid? parentId = null);
    void RemoveObject(Guid id);
    void UpdateObject(TowerObjectModel model);
    void ReparentObject(Guid id, Guid newParentId);
}

public class TowerRepo : IEditableTowerRepo
{
    public IAutoChannel AutoChannel => autoChannel;
    private readonly AutoChannel autoChannel = new AutoChannel();

    public IAutoValue<ITowerModel?> CurrentTower => currentTower;
    private readonly AutoValue<ITowerModel?> currentTower = new AutoValue<ITowerModel?>(null);

    private bool disposedValue;

    public void LoadTower(ITowerModel towerModel) => currentTower.Value = towerModel;

    public void UnloadTower() => currentTower.Value = null;

    public void AddObject(TowerObjectModel model, Guid? parentId = null)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        scene.AddTowerObject(model, parentId);

        autoChannel.Send(new ITowerRepo.ObjectAdded(model));
    }

    public void RemoveObject(Guid id)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        scene.RemoveTowerObject(id);

        autoChannel.Send(new ITowerRepo.ObjectRemoved(id));
    }

    public void UpdateObject(TowerObjectModel model)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        Guid? parentId = scene.GetParent(model.Id);
        scene.RemoveTowerObject(model.Id);
        scene.AddTowerObject(model, parentId);

        autoChannel.Send(new ITowerRepo.ObjectChanged(model));
    }

    public void ReparentObject(Guid id, Guid newParentId)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        scene.SetParent(id, newParentId);

        autoChannel.Send(new ITowerRepo.ObjectReparented(id, newParentId));
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                autoChannel.Dispose();
                currentTower.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion Internals
}
