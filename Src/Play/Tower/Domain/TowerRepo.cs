using System;
using Chickensoft.Sync.Primitives;
using Godot;
using Jomolith.Game.Domain;

namespace Jomolith.Play.Tower.Domain;

public interface ITowerRepo : IDisposable
{
    /// <summary>Mouse captured status.</summary>
    IAutoValue<bool> IsMouseCaptured { get; }

    /// <summary>Pause status.</summary>
    IAutoValue<bool> IsPaused { get; }

    /// <summary>Player's position in global coordinates.</summary>
    IAutoValue<Vector3> PlayerGlobalPosition { get; }

    /// <summary>Camera's global transform basis.</summary>
    IAutoValue<Basis> CameraBasis { get; }

    /// <summary>Camera's global forward direction vector.</summary>
    Vector3 GlobalCameraDirection { get; }

    /// <summary>Pauses the game and releases the mouse.</summary>
    void Pause();

    /// <summary>Resumes the game and recaptures the mouse.</summary>
    void Resume();

    void SetIsMouseCaptured(bool isMouseCaptured);

    void SetPlayerGlobalPosition(Vector3 playerGlobalPosition);

    void SetCameraBasis(Basis cameraBasis);
}

public class TowerRepo : ITowerRepo
{
    public IAutoValue<bool> IsMouseCaptured => isMouseCaptured;
    private readonly AutoValue<bool> isMouseCaptured;

    public IAutoValue<bool> IsPaused => isPaused;
    private readonly AutoValue<bool> isPaused;

    public IAutoValue<Vector3> PlayerGlobalPosition => playerGlobalPosition;
    private readonly AutoValue<Vector3> playerGlobalPosition;

    public IAutoValue<Basis> CameraBasis => cameraBasis;
    private readonly AutoValue<Basis> cameraBasis;

    public Vector3 GlobalCameraDirection => -cameraBasis.Value.Z;

    private bool disposedValue;

    public TowerRepo()
    {
        isMouseCaptured = new AutoValue<bool>(false);
        isPaused = new AutoValue<bool>(false);
        playerGlobalPosition = new AutoValue<Vector3>(Vector3.Zero);
        cameraBasis = new AutoValue<Basis>(Basis.Identity);
    }

    public void Pause()
    {
        isPaused.Value = true;
    }

    public void Resume()
    {
        isPaused.Value = false;
    }

    public void SetIsMouseCaptured(bool isMouseCaptured)
    {
        this.isMouseCaptured.Value = isMouseCaptured;
    }

    public void SetPlayerGlobalPosition(Vector3 playerGlobalPosition)
    {
        this.playerGlobalPosition.Value = playerGlobalPosition;
    }

    public void SetCameraBasis(Basis cameraBasis)
    {
        this.cameraBasis.Value = cameraBasis;
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
                playerGlobalPosition.Dispose();
                cameraBasis.Dispose();
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