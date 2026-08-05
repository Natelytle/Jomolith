using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu.State;

namespace Jomolith.Menu.Screens.Main;

[Meta(typeof(IAutoNode))]
public partial class MainMenu : Control, IScreen
{
    public bool ShowFooter => false;

    [Signal]
    public delegate void PlayButtonPressedEventHandler();

    [Signal]
    public delegate void SettingsButtonPressedEventHandler();

    public override void _Notification(int what) => this.Notify(what);

    [Export] private Button playButton { get; set; } = null!;
    [Export] private Button settingsButton { get; set; } = null!;

    public void OnReady()
    {
        playButton.Pressed += OnPlayPressed;
        settingsButton.Pressed += OnSettingsPressed;
    }

    public void OnExitTree()
    {
        playButton.Pressed -= OnPlayPressed;
        settingsButton.Pressed -= OnSettingsPressed;
    }

    public void OnEnter() { }

    public void OnExit() { }

    private void OnPlayPressed() => EmitSignal(SignalName.PlayButtonPressed);
    private void OnSettingsPressed() => EmitSignal(SignalName.SettingsButtonPressed);
}
