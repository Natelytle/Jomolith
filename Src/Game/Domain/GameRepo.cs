using System;

namespace Jomolith.Game.Domain;

/// <summary>
///     Pure application game logic repository shared between view-specific logic
///     blocks.
/// </summary>
public interface IGameRepo : IDisposable
{
    public event Action? MainMenuEntered;
    public event Action? TowerEntered;
    public event Action? TowerExited;

    public void OnMainMenuEntered();
    public void OnEnterTower();
}

/// <summary>
///     Pure application game logic repository — shared between view-specific logic
///     blocks.
/// </summary>
public class GameRepo : IGameRepo
{
    public event Action? MainMenuEntered;
    public event Action? TowerEntered;
    public event Action? TowerExited;

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
