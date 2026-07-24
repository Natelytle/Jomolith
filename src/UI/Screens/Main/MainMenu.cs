using Godot;
using Jomolith.UI.State;

namespace Jomolith.UI.Screens.Main;

public partial class MainMenu : Control, IScreen
{
    private UILogic uiLogic = null!;

    [Export] private Button playButton { get; set; } = null!;
    [Export] private Button settingsButton { get; set; } = null!;

    public void OnEnter(UILogic logic)
    {
        uiLogic = logic;

        playButton.Pressed += OnPlayPressed;
        settingsButton.Pressed += OnSettingsPressed;
    }

    public void OnExit()
    {
        playButton.Pressed -= OnPlayPressed;
        settingsButton.Pressed -= OnSettingsPressed;
    }

    private void OnPlayPressed() => uiLogic.Input(new UIState.Input.ToTowerSelect());
    private void OnSettingsPressed() => uiLogic.Input(new UIState.Input.ToSettings());
}
