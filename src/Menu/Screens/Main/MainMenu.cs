using Godot;
using Jomolith.Menu.State;

namespace Jomolith.Menu.Screens.Main;

public partial class MainMenu : Control, IScreen
{
    private MenuLogic menuLogic = null!;

    [Export] private Button playButton { get; set; } = null!;
    [Export] private Button settingsButton { get; set; } = null!;

    public void OnEnter(MenuLogic logic)
    {
        menuLogic = logic;

        playButton.Pressed += OnPlayPressed;
        settingsButton.Pressed += OnSettingsPressed;
    }

    public void OnExit()
    {
        playButton.Pressed -= OnPlayPressed;
        settingsButton.Pressed -= OnSettingsPressed;
    }

    private void OnPlayPressed() => menuLogic.Input(new MenuState.Input.ToTowerSelect());
    private void OnSettingsPressed() => menuLogic.Input(new MenuState.Input.ToSettings());
}
