using System;
using Chickensoft.Sync.Primitives;
using System.Numerics;

namespace Jomolith.Gameplay.Domain;

public interface IGameplayRepo : IDisposable
{
    IAutoChannel AutoChannel { get; }

    IAutoValue<bool> IsMouseCaptured { get; }

    readonly record struct GameplayStarted;

    void OnGameplayStarted();

    void SetIsMouseCaptured(bool captured);
}

public class GameplayRepo : IGameplayRepo
{
    public IAutoChannel AutoChannel => autoChannel;
    private readonly AutoChannel autoChannel = new();

    public IAutoValue<bool> IsMouseCaptured => isMouseCaptured;
    private readonly AutoValue<bool> isMouseCaptured = new(false);

    public IAutoValue<bool> IsPaused => isPaused;
    private readonly AutoValue<bool> isPaused = new(false);

    private bool disposedValue;

    public void OnGameplayStarted()
    {
        autoChannel.Send(new IGameplayRepo.GameplayStarted());
    }

    public void SetIsMouseCaptured(bool captured)
    {
        isMouseCaptured.Value = captured;
    }

    #region Internals

    protected void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
                // Dispose managed objects.
                autoChannel.Dispose();
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
