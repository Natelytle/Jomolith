using System;
using Chickensoft.Sync.Primitives;

namespace Jomolith.App.Domain;

/// <summary>
///     Pure application game logic repository shared between view-specific logic blocks.
/// </summary>
public interface IAppRepo : IDisposable
{
    IAutoChannel AutoChannel { get; }

    readonly record struct MainMenuEntered;
    readonly record struct TowerEntered;
    readonly record struct TowerExited;

    public void OnMainMenuEntered();
    public void OnEnterTower();
}

/// <summary>
///     Pure application game logic repository — shared between view-specific logic blocks.
/// </summary>
public class AppRepo : IAppRepo
{
    public IAutoChannel AutoChannel => autoChannel;
    private readonly AutoChannel autoChannel = new AutoChannel();

    private bool disposedValue;

    public void OnMainMenuEntered() => autoChannel.Send(new IAppRepo.MainMenuEntered());

    public void OnEnterTower() => autoChannel.Send(new IAppRepo.TowerEntered());

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                AutoChannel.Dispose();
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
