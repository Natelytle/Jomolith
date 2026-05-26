using System;
using Jomolith.Utils.Towers;

namespace Jomolith.App.Domain;

/// <summary>
///     Pure application game logic repository shared between view-specific logic blocks.
/// </summary>
public interface IAppRepo : IDisposable
{
    public event Action? MainMenuEntered;
    public event Action? TowerEntered;
    public event Action? TowerExited;

    public void OnMainMenuEntered();
    public void OnEnterTower();
}

/// <summary>
///     Pure application game logic repository — shared between view-specific logic blocks.
/// </summary>
public class AppRepo : IAppRepo
{
    public event Action? MainMenuEntered;
    public event Action? TowerEntered;
    public event Action? TowerExited;

    public WorkingTower Tower { get; private set; } = null!;

    private bool disposedValue;

    public void OnMainMenuEntered()
    {
        MainMenuEntered?.Invoke();
    }

    public void OnEnterTower()
    {
        TowerEntered?.Invoke();
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                MainMenuEntered = null;
                TowerEntered = null;
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
