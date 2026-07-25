using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu.State;

namespace Jomolith.Menu.Screens.Main;

[Meta(typeof(IAutoNode))]
public partial class MainMenu : Control, IScreen
{
    public bool ShowFooter => false;

    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    private IMenuLogic menuLogic => this.DependOn<IMenuLogic>();

    [Export] private Button playButton { get; set; } = null!;
    [Export] private Button settingsButton { get; set; } = null!;

    public void OnEnter()
    {
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
