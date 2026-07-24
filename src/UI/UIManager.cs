using System;
using System.Collections.Generic;
using Chickensoft.GodotNodeInterfaces;
using Godot;
using Jomolith.UI.Components;
using Jomolith.UI.State;

namespace Jomolith.UI;

public partial class UIManager : Control
{
    [Signal]
    public delegate void QuitRequestedEventHandler();

    private UILogic logic = null!;
    private IScreen? currentScreen;

    private Dictionary<Type, PackedScene> screenScenes = null!;

    [Export] private Control screenContainer { get; set; } = null!;

    [Export] private PackedScene mainMenuScene { get; set; } = null!;
    [Export] private PackedScene towerSelect { get; set; } = null!;
    [Export] private PackedScene player { get; set; } = null!;
    [Export] private PackedScene settingsMenu { get; set; } = null!;

    [Export] private Footer footer { get; set; } = null!;
    [Export] private ExitPrompt exitPrompt { get; set; } = null!;

    private bool canGoBack;

    public override void _Ready()
    {
        screenScenes = new Dictionary<Type, PackedScene>
        {
            [typeof(UIState.MainMenu)] = mainMenuScene,
            [typeof(UIState.TowerSelect)] = towerSelect,
            [typeof(UIState.Play)] = player,
            [typeof(UIState.Settings)] = settingsMenu,
        };

        logic = new UILogic();

        logic.Bind()
            .OnOutput<UIState.Output.ScreenChanged>((in o) =>
            {
                SwapScreen(o.NewScreen);
                canGoBack = o.CanGoBack;
            })
            .OnOutput<UIState.Output.ExitPromptVisible>((in o) => exitPrompt.Visible = o.Visible)
            .OnOutput<UIState.Output.QuitGame>((in _) => EmitSignal(SignalName.QuitRequested));

        footer.BackPressed += () => logic.Input(new UIState.Input.Back());

        logic.Start<UIState.MainMenu>();
    }

    public void SwapScreen(Type screenType)
    {
        if (currentScreen is not null)
        {
            currentScreen.OnExit();
            currentScreen.QueueFree();
        }

        currentScreen = (IScreen)screenScenes[screenType].Instantiate();

        screenContainer.AddChildEx(currentScreen!);
        currentScreen.OnEnter(logic);
    }

    public override void _UnhandledInput(InputEvent @event) {
        if (!@event.IsActionPressed("ui_cancel")) return; // Escape, by default

        if (exitPrompt.Visible)
            logic.Input(new UIState.Input.ExitCancelled());
        else if (canGoBack)
            logic.Input(new UIState.Input.Back());
        else
            logic.Input(new UIState.Input.RequestExit());

        GetViewport().SetInputAsHandled();
    }
}
