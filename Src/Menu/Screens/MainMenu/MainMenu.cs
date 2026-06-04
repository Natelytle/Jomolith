
using Godot;

namespace Jomolith.Menu.Screens.MainMenu;

public interface IMainMenu : IScreen
{
    event MainMenu.PlayPressedEventHandler PlayPressed;
    event MainMenu.EditPressedEventHandler EditPressed;
}

public partial class MainMenu : Control, IMainMenu
{
    [Export] public Button PlayButton { get; set; } = null!;
    [Export] public Button EditButton { get; set; } = null!;

    [Signal]
    public delegate void PlayPressedEventHandler();

    [Signal]
    public delegate void EditPressedEventHandler();

    public override void _Ready()
    {
        PlayButton.Pressed += OnPlayButtonPressed;
        EditButton.Pressed += OnEditButtonPressed;
    }

    private void OnPlayButtonPressed() => EmitSignal(SignalName.PlayPressed);
    private void OnEditButtonPressed() => EmitSignal(SignalName.EditPressed);
}
