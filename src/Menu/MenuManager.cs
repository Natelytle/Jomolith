using System;
using System.Collections.Generic;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu.Components;
using Jomolith.Menu.State;

namespace Jomolith.Menu;

public interface IMenuManager : IControl
{
    event MenuManager.QuitRequestedEventHandler QuitRequested;
}

[Meta(typeof(IAutoNode))]
public partial class MenuManager : Control, IMenuManager
{
    public override void _Notification(int what) => this.Notify(what);

    private const string main_menu_scene_path = "res://src/Menu/Screens/Main/MainMenu.tscn";
    private const string tower_select_scene_path = "res://src/Menu/Screens/Select/TowerSelect.tscn";
    private const string play_scene_path = "res://src/Menu/Screens/Play/Player.tscn";
    private const string settings_screen_path = "res://src/Menu/Screens/Settings/SettingsMenu.tscn";

    [Signal]
    public delegate void QuitRequestedEventHandler();

    private MenuLogic logic = null!;
    private IScreen? currentScreen;

    private Dictionary<Type, string> screenScenePaths = null!;

    [Node("%ScreenContainer")]
    private IControl screenContainer { get; set; } = null!;

    [Node("%Footer")]
    private IFooter footer { get; set; } = null!;

    [Node("%ExitPrompt")]
    private IExitPrompt exitPrompt { get; set; } = null!;

    private bool canGoBack;

    public void Setup()
    {
        logic = new MenuLogic();
    }

    public void OnResolved()
    {
        screenScenePaths = new Dictionary<Type, string>
        {
            [typeof(MenuState.MainMenu)] = main_menu_scene_path,
            [typeof(MenuState.TowerSelect)] = tower_select_scene_path,
            [typeof(MenuState.Play)] = play_scene_path,
            [typeof(MenuState.Settings)] = settings_screen_path,
        };

        logic.Bind()
            .OnOutput<MenuState.Output.ScreenChanged>((in o) =>
            {
                SwapScreen(o.NewScreen);
                canGoBack = o.CanGoBack;
            })
            .OnOutput<MenuState.Output.ExitPromptVisible>((in o) => exitPrompt.Visible = o.Visible)
            .OnOutput<MenuState.Output.QuitGame>((in _) => EmitSignal(SignalName.QuitRequested));

        footer.BackPressed += () => logic.Input(new MenuState.Input.Back());

        logic.Start<MenuState.MainMenu>();
    }

    public void SwapScreen(Type screenType)
    {
        if (currentScreen is not null)
        {
            currentScreen.OnExit();
            currentScreen.QueueFree();
        }

        currentScreen = (IScreen)GD.Load<PackedScene>(screenScenePaths[screenType]).Instantiate();

        screenContainer.AddChildEx(currentScreen!);
        currentScreen.OnEnter(logic);
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (!@event.IsActionPressed("ui_cancel")) return; // Escape, by default

        if (exitPrompt.Visible)
            logic.Input(new MenuState.Input.ExitCancelled());
        else if (canGoBack)
            logic.Input(new MenuState.Input.Back());
        else
            logic.Input(new MenuState.Input.RequestExit());

        GetViewport().SetInputAsHandled();
    }
}
