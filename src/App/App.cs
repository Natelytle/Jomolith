using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.App.State;
using Jomolith.Menu;

namespace Jomolith.App;

[Meta(typeof(IAutoNode))]
public partial class App : Node, IProvide<IAppRepo>
{
    public override void _Notification(int what) => this.Notify(what);

    IAppRepo IProvide<IAppRepo>.Value() => appRepo;

    private IAppRepo appRepo { get; set; } = null!;
    private IAppLogic appLogic { get; set; } = null!;

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

        menuScene.QuitRequested += () => GetTree().Quit();
    }
}
