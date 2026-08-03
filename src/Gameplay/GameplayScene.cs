using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Gameplay.Domain;
using Jomolith.Gameplay.State;
using Jomolith.Towers.Domain.Models;
using Jomolith.Towers.Factory;
using Jomolith.Gameplay.Player;

namespace Jomolith.Gameplay;

public interface IGameplayScene : IControl, IProvide<IGameplayRepo>;

[Meta(typeof(IAutoNode))]
public partial class GameplayScene : Control, IGameplayScene
{
    public override void _Notification(int what) => this.Notify(what);

    private const string player_scene_path = "res://src/Gameplay/Player/Player.tscn";

    [Dependency]
    private IAppRepo appRepo => this.DependOn<IAppRepo>();

    IGameplayRepo IProvide<IGameplayRepo>.Value() => gameplayRepo;

    private IGameplayRepo gameplayRepo { get; set; } = null!;
    private IGameplayLogic gameplayLogic { get; set; } = null!;

    private readonly TowerBuilder towerBuilder = new();

    [Node("%World")]
    private INode3D world { get; set; } = null!;

    private Node3D? towerNode;
    private IPlayer? player { get; set; }

    public void Setup()
    {
        gameplayRepo = new GameplayRepo();
        gameplayLogic = new GameplayLogic();
    }

    public void OnResolved()
    {
        gameplayLogic.Set(gameplayRepo);
        gameplayLogic.Set(appRepo);

        gameplayLogic.Bind()
            .OnOutput((in GameplayState.Output.Load o) => load(o.Tower))
            .OnOutput((in GameplayState.Output.Unload _) => unload())
            .OnOutput((in GameplayState.Output.SetMouseCaptureMode output) =>
                Input.SetMouseMode(output.Captured ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible))
            .OnOutput((in GameplayState.Output.SetPaused output) =>
                CallDeferred(nameof(setPauseMode), output.Paused));

        this.Provide();

        gameplayLogic.Start<GameplayState.Unloaded>();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        base._UnhandledInput(@event);
    }

    private void load(TowerModel model)
    {
        towerNode = towerBuilder.BuildTower(model);
        world.AddChild(towerNode);

        player = (IPlayer)GD.Load<PackedScene>(player_scene_path).Instantiate();
        player.Position = new Vector3(model.SpawnPosition.X, model.SpawnPosition.Y, model.SpawnPosition.Z);
        world.AddChildEx(player);

        gameplayLogic.Input(new GameplayState.Input.LoadComplete());
    }

    private void unload()
    {
        towerNode?.QueueFree();
        player?.QueueFree();

        towerNode = null;
    }

    private void setPauseMode(bool paused) => GetTree().Paused = paused;
}
