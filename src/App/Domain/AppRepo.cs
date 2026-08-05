using Chickensoft.Sync.Primitives;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.App.Domain;

public interface IAppRepo
{
    IAutoChannel AutoChannel { get; }

    readonly record struct EnteringTower(TowerModel Tower);
    readonly record struct ExitingTower;

    void OnEnteringTower(TowerModel tower);
    void OnExitingTower();
}

public class AppRepo : IAppRepo
{
    private readonly AutoChannel autoChannel = new();
    public IAutoChannel AutoChannel => autoChannel;

    public void OnEnteringTower(TowerModel tower)
    {
        autoChannel.Send(new IAppRepo.EnteringTower(tower));
    }

    public void OnExitingTower()
    {
        autoChannel.Send(new IAppRepo.ExitingTower());
    }
}
