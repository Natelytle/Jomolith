using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Menu.Screens.Components.Footer;

public interface IFooter : IControl
{
    event Footer.BackButtonPressedEventHandler BackButtonPressed;
}

public partial class Footer : Control, IFooter
{
    #region Nodes

    [Export] public Button BackButton { get; set; } = null!;

    #endregion

    #region Signals

    [Signal] public delegate void BackButtonPressedEventHandler();

    #endregion

    public override void _Ready()
    {
        BackButton.Pressed += OnBackButtonPressed;
    }

    public override void _ExitTree()
    {
        BackButton.Pressed -= OnBackButtonPressed;
    }

    public void OnBackButtonPressed() => EmitSignal(SignalName.BackButtonPressed);
}
