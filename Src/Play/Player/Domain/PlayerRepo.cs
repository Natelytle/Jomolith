using System;
using Chickensoft.Sync.Primitives;
using Godot;

namespace Jomolith.Play.Player.Domain;

public interface IPlayerRepo : IDisposable
{
    /// <summary>Rotation locked status.</summary>
    IAutoValue<bool> IsPlayerRotationLocked { get; }

    /// <summary>Player's position in global coordinates.</summary>
    IAutoValue<Vector3> PlayerGlobalPosition { get; }

    /// <summary>Player's global transform basis.</summary>
    IAutoValue<Basis> PlayerBasis { get; }

    /// <summary>Camera's global transform basis.</summary>
    IAutoValue<Basis> CameraBasis { get; }

    void SetIsPlayerRotationLocked(bool isLocked);

    void SetPlayerGlobalPosition(Vector3 playerGlobalPosition);

    void SetPlayerBasis(Basis playerBasis);

    void SetCameraBasis(Basis cameraBasis);
}

public class PlayerRepo : IPlayerRepo
{
    public IAutoValue<bool> IsPlayerRotationLocked => isPlayerRotationLocked;
    private readonly AutoValue<bool> isPlayerRotationLocked;

    public IAutoValue<Vector3> PlayerGlobalPosition => playerGlobalPosition;
    private readonly AutoValue<Vector3> playerGlobalPosition;

    public IAutoValue<Basis> PlayerBasis => playerBasis;
    private readonly AutoValue<Basis> playerBasis;

    public IAutoValue<Basis> CameraBasis => cameraBasis;
    private readonly AutoValue<Basis> cameraBasis;

    private bool disposedValue;

    public PlayerRepo()
    {
        isPlayerRotationLocked = new AutoValue<bool>(false);
        playerGlobalPosition = new AutoValue<Vector3>(Vector3.Zero);
        playerBasis = new AutoValue<Basis>(Basis.Identity);
        cameraBasis = new AutoValue<Basis>(Basis.Identity);
    }

    public void SetIsPlayerRotationLocked(bool isPlayerRotationLocked)
    {
        this.isPlayerRotationLocked.Value = isPlayerRotationLocked;
    }

    public void SetPlayerGlobalPosition(Vector3 playerGlobalPosition)
    {
        this.playerGlobalPosition.Value = playerGlobalPosition;
    }

    public void SetPlayerBasis(Basis playerBasis)
    {
        this.playerBasis.Value = playerBasis;
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
                isPlayerRotationLocked.Dispose();
                playerGlobalPosition.Dispose();
                playerBasis.Dispose();
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
