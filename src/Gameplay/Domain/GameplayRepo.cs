using Chickensoft.Sync.Primitives;
using System.Numerics;

namespace Jomolith.Gameplay.Domain;

public interface IGameplayRepo
{
    IAutoChannel AutoChannel { get; }

    readonly record struct GameplayStarted(Vector3 SpawnPosition);

    void OnGameplayStarted(Vector3 spawnPosition);
}

public class GameplayRepo : IGameplayRepo
{
    public IAutoChannel AutoChannel => autoChannel;
    private AutoChannel autoChannel = new();

    public void OnGameplayStarted(Vector3 spawnPosition)
    {
        autoChannel.Send(new IGameplayRepo.GameplayStarted(spawnPosition));
    }
}
