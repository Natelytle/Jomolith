using System.Linq;
using Godot;
using Jomolith.Towers.Factory;
using Jomolith.Towers.Services;
using Jomolith.UI;

namespace Jomolith;

public partial class Main : Node
{
    [Export] private UIManager uiManager = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        var localTowerRepo = new LocalTowerRepository();
        var tower = localTowerRepo.LoadAllTowers().First();

        TowerBuilder towerBuilder = new TowerBuilder();
        var towerNode = towerBuilder.BuildTower(tower);

        AddChild(towerNode);

        uiManager.QuitRequested += () => GetTree().Quit();
    }
}
