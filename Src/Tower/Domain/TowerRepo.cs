
using System;
using Chickensoft.Sync.Primitives;
using Jomolith.Tower.Core;
using Jomolith.Tower.Core.Objects;

namespace Jomolith.Tower.Domain;

public interface ITowerRepo
{
    IAutoValue<ITowerModel?> CurrentTower { get; }

    event Action<TowerObjectModel>? ObjectAdded;
    event Action<Guid>? ObjectRemoved;
    event Action<TowerObjectModel>? ObjectChanged;
    event Action<Guid, Guid> ObjectReparented;
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
    public IAutoValue<ITowerModel?> CurrentTower => currentTower;
    private readonly AutoValue<ITowerModel?> currentTower;

    public event Action<TowerObjectModel>? ObjectAdded;
    public event Action<Guid>? ObjectRemoved;
    public event Action<TowerObjectModel>? ObjectChanged;
    public event Action<Guid, Guid>? ObjectReparented;

    private bool disposedValue;

    public TowerRepo()
    {
        currentTower = new AutoValue<ITowerModel?>(null);
    }

    public void LoadTower(ITowerModel towerModel) => currentTower.Value = towerModel;

    public void UnloadTower() => currentTower.Value = null;

    public void AddObject(TowerObjectModel model, Guid? parentId = null)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        scene.AddTowerObject(model, parentId);

        ObjectAdded?.Invoke(model);
    }

    public void RemoveObject(Guid id)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        scene.RemoveTowerObject(id);

        ObjectRemoved?.Invoke(id);
    }

    public void UpdateObject(TowerObjectModel model)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        Guid? parentId = scene.GetParent(model.Id);
        scene.RemoveTowerObject(model.Id);
        scene.AddTowerObject(model, parentId);

        ObjectChanged?.Invoke(model);
    }

    public void ReparentObject(Guid id, Guid newParentId)
    {
        TowerSceneModel? scene = currentTower.Value?.Scene;

        if (scene is null) return;

        scene.SetParent(id, newParentId);

        ObjectReparented?.Invoke(id, newParentId);
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
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
