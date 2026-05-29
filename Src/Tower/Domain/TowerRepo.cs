
using System;
using Chickensoft.Sync.Primitives;
using Jomolith.Tower.Core;

namespace Jomolith.Tower.Domain;

public interface ITowerRepo
{
    IAutoValue<ITowerModel?> CurrentTower { get; }

    void LoadTower(ITowerModel towerModel);
    void UnloadTower();
}

public class TowerRepo : ITowerRepo
{
    public IAutoValue<ITowerModel?> CurrentTower => currentTower;
    private readonly AutoValue<ITowerModel?> currentTower;

    private bool disposedValue;

    public TowerRepo()
    {
        currentTower = new AutoValue<ITowerModel?>(null);
    }

    public void LoadTower(ITowerModel towerModel) => currentTower.Value = towerModel;

    public void UnloadTower() => currentTower.Value = null;

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
