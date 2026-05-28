using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Core;
using Jomolith.Play.Gameplay.Domain;
using Jomolith.Play.Gameplay.State;
using Jomolith.Play.Player;
using Jomolith.Utils.Towers;

namespace Jomolith.Play.Gameplay;

public interface IGameplay : INode3D, IProvide<IGameplayRepo>
{
    IGameplayLogic GameplayLogic { get; }

    public void LoadTower(ITowerModel towerModel);

    event Gameplay.TowerLoadedEventHandler? TowerLoaded;
}

[Meta(typeof(IAutoNode))]
public partial class Gameplay : Node3D, IGameplay
{
    public override void _Notification(int what) => this.Notify(what);

    #region Loading

    [Signal]
    public delegate void TowerLoadedEventHandler();

    #endregion

    #region Dependencies

    [Dependency] public IAppRepo GameRepo => this.DependOn<IAppRepo>();

    #endregion

    #region Provisions

    IGameplayRepo IProvide<IGameplayRepo>.Value() => GameplayRepo;

    #endregion

    #region State

    public IGameplayRepo GameplayRepo { get; set; } = null!;

    public IGameplayLogic GameplayLogic { get; set; } = null!;

    public GameplayLogic.IBinding GameplayBinding { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public IWorkingTower Tower { get; set; } = null!;

    [Node] public IPlayer Player { get; set; } = null!;

    #endregion

    public void Setup()
    {
        GameplayRepo = new GameplayRepo();
        GameplayLogic = new GameplayLogic();

        GameplayLogic.Set(GameplayRepo);
    }

    public void OnResolved()
    {
        GameplayBinding = GameplayLogic.Bind();

        GameplayBinding
            .Handle((in GameplayLogic.Output.SetMouseCaptureMode output) =>
            {
                Input.SetMouseMode(output.IsMouseCaptured ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
            })
            .Handle((in GameplayLogic.Output.SetPaused output) =>
            {
                CallDeferred(nameof(setPauseMode), output.IsPaused);
            });

        this.Provide();

        GameplayLogic.Start();

        Tower.Initialize();
    }

    // TODO: Make async
    public void LoadTower(ITowerModel towerModel)
    {
        Tower.Load(towerModel);

        finishedLoadingTower();
    }

    private void finishedLoadingTower() => EmitSignal(SignalName.TowerLoaded);

    private void setPauseMode(bool pause) => GetTree().Paused = pause;
}
