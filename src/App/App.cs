using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.App.Domain;
using Jomolith.App.State;
using Jomolith.Gameplay;
using Jomolith.Menu;
using Jomolith.Settings;

namespace Jomolith.App;

public interface IApp : INode, IProvide<IAppRepo>, IProvide<ISettingsService>;

[Meta(typeof(IAutoNode))]
public partial class App : Node, IApp
{
    public override void _Notification(int what) => this.Notify(what);

    IAppRepo IProvide<IAppRepo>.Value() => AppRepo;
    ISettingsService IProvide<ISettingsService>.Value() => SettingsService;

    public IAppRepo AppRepo { get; set; } = null!;
    public IAppLogic AppLogic { get; set; } = null!;
    public LogicBlock.Binding AppBinding { get; set; } = null!;
    public ISettingsService SettingsService { get; set; } = null!;

    [Node("%GameplayScene")] public IGameplayScene GameplayScene { get; set; } = null!;

    [Node("%MenuScene")] public IMenuScene MenuScene { get; set; } = null!;

    public void Setup()
    {
        AppRepo = new AppRepo();
        AppLogic = new AppLogic();

        SettingsService = new SettingsService();
    }

    public void OnResolved()
    {
        AppLogic.Set(AppRepo);

        SettingsService.ApplyBindings(SettingsService.Load().KeyBindings);

        AppBinding = AppLogic.Bind()
            .OnOutput((in AppState.Output.SetGameVisibility o) => GameplayScene.Visible = o.Visible)
            .OnOutput((in AppState.Output.SetMenuVisibility o) => MenuScene.Visible = o.Visible);

        MenuScene.QuitRequested += quitRequested;

        this.Provide();

        AppLogic.Start<AppState.InMenus>();
    }

    public void OnExitTree()
    {
        AppLogic.Stop();
        AppBinding.Dispose();
        AppRepo.Dispose();

        MenuScene.QuitRequested -= quitRequested;
    }

    private void quitRequested() => GetTree().Quit();
}
