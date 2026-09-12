using System;
using Chickensoft.Sync.Primitives;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.App.Domain;

public interface IAppRepo : IDisposable
{
    IAutoChannel AutoChannel { get; }

    readonly record struct EnteringTower(TowerModel Tower);
    readonly record struct ExitingTower;

    void OnEnteringTower(TowerModel tower);
    void OnExitingTower();
}

public class AppRepo : IAppRepo
{
    private readonly AutoChannel autoChannel = new();
    public IAutoChannel AutoChannel => autoChannel;

    private bool disposedValue;

    public void OnEnteringTower(TowerModel tower)
    {
        autoChannel.Send(new IAppRepo.EnteringTower(tower));
    }

    public void OnExitingTower()
    {
        autoChannel.Send(new IAppRepo.ExitingTower());
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                autoChannel.Dispose();
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    #endregion Internals
}
