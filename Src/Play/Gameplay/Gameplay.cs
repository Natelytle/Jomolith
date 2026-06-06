using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Play.Gameplay.Domain;
using Jomolith.Play.Gameplay.State;
using Jomolith.Play.Gameplay.State.States;
using Jomolith.Play.Player;
using Jomolith.Tower;
using Jomolith.Tower.Domain;

namespace Jomolith.Play.Gameplay;

// ReSharper disable once PossibleInterfaceMemberAmbiguity
public interface IGameplay : INode3D, IProvide<IGameplayRepo>, IProvide<ITowerRepo>
{
    IGameplayLogic GameplayLogic { get; }
}

[Meta(typeof(IAutoNode))]
public partial class Gameplay : Node3D, IGameplay
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] public IAppRepo AppRepo => this.DependOn<IAppRepo>();

    #endregion

    #region Provisions

    IGameplayRepo IProvide<IGameplayRepo>.Value() => GameplayRepo;
    ITowerRepo IProvide<ITowerRepo>.Value() => TowerRepo;

    #endregion

    #region State

    public IGameplayRepo GameplayRepo { get; set; } = null!;

    public IGameplayLogic GameplayLogic { get; set; } = null!;

    public ITowerRepo TowerRepo { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public ITower Tower { get; set; } = null!;

    [Node] public IPlayer Player { get; set; } = null!;

    #endregion

    public void Setup()
    {
        GameplayRepo = new GameplayRepo();
        GameplayLogic = new GameplayLogic();

        TowerRepo = new TowerRepo();

        GameplayLogic.Set(GameplayRepo);
    }

    public void OnResolved()
    {
        using var binding = GameplayLogic.Bind();

        binding
            .OnOutput((in GameplayLogic.Outputs.SetMouseCaptureMode output) =>
            {
                Input.SetMouseMode(output.IsMouseCaptured ? Input.MouseModeEnum.Captured : Input.MouseModeEnum.Visible);
            })
            .OnOutput((in GameplayLogic.Outputs.SetPaused output) =>
            {
                CallDeferred(nameof(setPauseMode), output.IsPaused);
            });

        this.Provide();

        GameplayLogic.Start<GameplayState>();
    }

    private void setPauseMode(bool pause) => GetTree().Paused = pause;
}
