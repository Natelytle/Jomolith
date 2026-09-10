
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Menu.Components;
using Jomolith.Menu.Screens.Main;
using Jomolith.Menu.Screens.Select;
using Jomolith.Menu.Screens.Settings;
using Jomolith.Menu.State;
using Jomolith.Towers.Domain.Models;

namespace Jomolith.Menu;

public interface IMenuScene : IControl
{
    event MenuScene.QuitRequestedEventHandler QuitRequested;
}

[Meta(typeof(IAutoNode))]
public partial class MenuScene : Control, IMenuScene
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    private IAppRepo appRepo => this.DependOn<IAppRepo>();

    [Signal]
    public delegate void QuitRequestedEventHandler();

    private MenuLogic menuLogic = null!;
    private IScreen? currentScreen;

    [Node("%MainMenu")]
    private MainMenu mainMenu { get; set; } = null!;

    [Node("%TowerSelect")]
    private TowerSelect towerSelect { get; set; } = null!;

    [Node("%SettingsMenu")]
    private SettingsMenu settingsMenu { get; set; } = null!;

    [Node("%ScreenContainer")]
    private IControl screenContainer { get; set; } = null!;

    [Node("%Footer")]
    private IFooter footer { get; set; } = null!;

    [Node("%ExitPrompt")]
    private IExitPrompt exitPrompt { get; set; } = null!;

    public void Setup()
    {
        menuLogic = new MenuLogic();

        mainMenu.PlayButtonPressed += onPlayButtonPressed;
        mainMenu.SettingsButtonPressed += onSettingsButtonPressed;
        towerSelect.TowerSelected += onTowerSelected;
        footer.BackPressed += onBackPressed;
    }

    public void OnResolved()
    {
        menuLogic.Set(appRepo);

        menuLogic.Bind()
            .OnOutput<MenuState.Output.ShowMainMenu>((in _) => SwapScreen(mainMenu))
            .OnOutput<MenuState.Output.ShowTowerSelect>((in _) => SwapScreen(towerSelect))
            .OnOutput<MenuState.Output.ShowSettings>((in _) => SwapScreen(settingsMenu))
            .OnOutput<MenuState.Output.ExitPromptVisible>((in o) => exitPrompt.Visible = o.Visible)
            .OnOutput<MenuState.Output.QuitGame>((in _) => EmitSignal(SignalName.QuitRequested));

        this.Provide();

        menuLogic.Start<MenuState.MainMenu>();
    }

    public void SwapScreen(IScreen screen)
    {
        if (ReferenceEquals(screen, currentScreen)) return;

        currentScreen?.OnExit();
        currentScreen?.Hide();

        currentScreen = screen;
        currentScreen.Show();

        currentScreen.OnEnter();
        footer.Visible = currentScreen.ShowFooter;
    }

    public override void _Input(InputEvent @event)
    {
        if (!Visible) return; // Only process input if the menu is visible.
        if (!@event.IsActionPressed("ui_cancel")) return;

        menuLogic.Input(new MenuState.Input.Back());

        GetViewport().SetInputAsHandled();
    }

    public override void _ExitTree()
    {
        mainMenu.PlayButtonPressed -= onPlayButtonPressed;
        mainMenu.SettingsButtonPressed -= onSettingsButtonPressed;
        towerSelect.TowerSelected -= onTowerSelected;
        footer.BackPressed -= onBackPressed;
    }

    private void onPlayButtonPressed() => menuLogic.Input(new MenuState.Input.ToTowerSelect());
    private void onSettingsButtonPressed() => menuLogic.Input(new MenuState.Input.ToSettings());
    private void onTowerSelected(TowerModel towerModel) => menuLogic.Input(new MenuState.Input.TowerSelected(towerModel));
    private void onBackPressed() => menuLogic.Input(new MenuState.Input.Back());
}
