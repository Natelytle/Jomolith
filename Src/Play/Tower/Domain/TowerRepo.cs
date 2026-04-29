using System;
using Chickensoft.Sync.Primitives;
using Godot;
using Jomolith.Game.Domain;

namespace Jomolith.Play.Tower.Domain;

public interface ITowerRepo : IDisposable
{
    /// <summary>Pause status.</summary>
    IAutoValue<bool> IsPaused { get; }

    /// <summary>Pauses the game and releases the mouse.</summary>
    void Pause();

    /// <summary>Resumes the game and recaptures the mouse.</summary>
    void Resume();
}

public class TowerRepo : ITowerRepo
{
    public IAutoValue<bool> IsPaused => isPaused;
    private readonly AutoValue<bool> isPaused;

    private bool disposedValue;

    public TowerRepo()
    {
        isPaused = new AutoValue<bool>(false);
    }

    public void Pause()
    {
        isPaused.Value = true;
    }

    public void Resume()
    {
        isPaused.Value = false;
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                isPaused.Dispose();
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