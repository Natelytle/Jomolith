using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Menu;

public interface IMainMenu : IControl
{
    event MainMenu.PlayTowerEventHandler PlayTower;
    event MainMenu.EditTowerEventHandler EditTower;
}

public partial class MainMenu : Control, IMainMenu
{
    [Export] public Button PlayButton { get; set; } = null!;
    [Export] public Button EditButton { get; set; } = null!;

    [Signal]
    public delegate void PlayTowerEventHandler();

    [Signal]
    public delegate void EditTowerEventHandler();

    public override void _Ready()
    {
        PlayButton.Pressed += OnPlayButtonPressed;
        EditButton.Pressed += OnEditButtonPressed;
    }

    private void OnPlayButtonPressed() => EmitSignal(SignalName.PlayTower);
    private void OnEditButtonPressed() => EmitSignal(SignalName.EditTower);
}
