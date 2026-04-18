using System;

namespace Jomolith.Game.Domain;

/// <summary>
///     Pure application game logic repository shared between view-specific logic
///     blocks.
/// </summary>
public interface IAppRepo : IDisposable
{
}

/// <summary>
///     Pure application game logic repository — shared between view-specific logic
///     blocks.
/// </summary>
public class GameRepo : IAppRepo
{
    private bool _disposedValue;
    public event Action? SplashScreenSkipped;
    public event Action? MainMenuEntered;
    public event Action? GameEntered;

    public void SkipSplashScreen()
    {
        SplashScreenSkipped?.Invoke();
    }

    public void OnMainMenuEntered()
    {
        MainMenuEntered?.Invoke();
    }

    public void OnEnterGame()
    {
        GameEntered?.Invoke();
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!_disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                SplashScreenSkipped = null;
                MainMenuEntered = null;
                GameEntered = null;
            }

            _disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion Internals
}