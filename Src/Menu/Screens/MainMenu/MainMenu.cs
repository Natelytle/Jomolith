using Godot;

namespace Jomolith.Menu.Screens.MainMenu;

public interface IMainMenu : IScreen
{
    event MainMenu.PlayPressedEventHandler PlayPressed;
    event MainMenu.EditPressedEventHandler EditPressed;
    event MainMenu.SettingsPressedEventHandler SettingsPressed;
}

public partial class MainMenu : Control, IMainMenu
{
    #region Nodes

    [Export] public Button PlayButton { get; set; } = null!;
    [Export] public Button EditButton { get; set; } = null!;
    [Export] public Button SettingsButton { get; set; } = null!;

    #endregion

    #region Signals

    [Signal] public delegate void PlayPressedEventHandler();
    [Signal] public delegate void EditPressedEventHandler();
    [Signal] public delegate void SettingsPressedEventHandler();

    #endregion

    public override void _Ready()
    {
        PlayButton.Pressed += OnPlayButtonPressed;
        EditButton.Pressed += OnEditButtonPressed;
        SettingsButton.Pressed += OnSettingsButtonPressed;
    }

    private void OnPlayButtonPressed() => EmitSignal(SignalName.PlayPressed);
    private void OnEditButtonPressed() => EmitSignal(SignalName.EditPressed);
    private void OnSettingsButtonPressed() => EmitSignal(SignalName.SettingsPressed);
}
