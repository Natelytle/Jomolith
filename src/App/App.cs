using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.App.State;
using Jomolith.Gameplay;
using Jomolith.Menu;

namespace Jomolith.App;

public interface IApp : INode, IProvide<IAppRepo>;

[Meta(typeof(IAutoNode))]
public partial class App : Node, IApp
{
    public override void _Notification(int what) => this.Notify(what);

    IAppRepo IProvide<IAppRepo>.Value() => appRepo;

    private IAppRepo appRepo { get; set; } = null!;
    private IAppLogic appLogic { get; set; } = null!;

    [Node("%GameplayScene")]
    private IGameplayScene gameplayScene { get; set; } = null!;

    [Node("%MenuScene")]
    private IMenuScene menuScene { get; set; } = null!;

    public void Setup()
    {
        appRepo = new AppRepo();
        appLogic = new AppLogic();
    }

    public void OnResolved()
    {
        appLogic.Set(appRepo);

        appLogic.Bind()
            .OnOutput((in AppState.Output.ShowGame _) => gameplayScene.Show())
            .OnOutput((in AppState.Output.HideGame _) => gameplayScene.Hide());

        menuScene.QuitRequested += () => GetTree().Quit();

        this.Provide();

        appLogic.Start<AppState.InMenus>();
    }
}
