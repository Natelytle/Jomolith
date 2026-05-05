using System;
using Chickensoft.Sync.Primitives;

namespace Jomolith.Play.Gameplay.Domain;

public interface IGameplayRepo : IDisposable
{
    /// <summary>
    /// Mouse captured status.
    /// </summary>
    IAutoValue<bool> IsMouseCaptured { get; }

    /// <summary>
    /// Pause status.
    /// </summary>
    IAutoValue<bool> IsPaused { get; }

    /// <summary>
    /// Sets the value of <see cref="IsMouseCaptured"/>.
    /// </summary>
    /// <param name="isMouseCaptured">The state to set.</param>
    void SetIsMouseCaptured(bool isMouseCaptured);

    /// <summary>
    /// Pauses the game and releases the mouse.
    /// </summary>
    void Pause();

    /// <summary>
    /// Resumes the game and recaptures the mouse.
    /// </summary>
    void Resume();
}

public class GameplayRepo : IGameplayRepo
{
    public IAutoValue<bool> IsMouseCaptured => isMouseCaptured;
    private readonly AutoValue<bool> isMouseCaptured;

    public IAutoValue<bool> IsPaused => isPaused;
    private readonly AutoValue<bool> isPaused;

    private bool disposedValue;

    public GameplayRepo()
    {
        isMouseCaptured = new AutoValue<bool>(false);
        isPaused = new AutoValue<bool>(false);
    }

    public void SetIsMouseCaptured(bool isMouseCaptured)
    {
        this.isMouseCaptured.Value = isMouseCaptured;
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
                isMouseCaptured.Dispose();
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
