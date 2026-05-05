using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Play.Gameplay.Domain;
using Jomolith.Play.Gameplay.State;

namespace Jomolith.Play.Gameplay;

public interface IGameplay : INode3D, IProvide<IGameplayRepo>
{
    IGameplayLogic GameplayLogic { get; }
}

[Meta(typeof(IAutoNode))]
public partial class Gameplay : Node3D, IGameplay
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    #endregion

    #region Provisions

    IGameplayRepo IProvide<IGameplayRepo>.Value() => GameplayRepo;

    #endregion

    #region State

    public IGameplayRepo GameplayRepo { get; set; } = null!;

    public IGameplayLogic GameplayLogic { get; set; } = null!;

    public GameplayLogic.IBinding GameplayBinding { get; set; } = null!;

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
    }

    private void setPauseMode(bool pause) => GetTree().Paused = pause;
}
