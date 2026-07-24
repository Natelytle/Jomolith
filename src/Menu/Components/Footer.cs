using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Menu.Components;

public interface IFooter : IControl
{
    event Footer.BackPressedEventHandler BackPressed;
}

public partial class Footer : Control, IFooter
{
    [Signal] public delegate void BackPressedEventHandler();

    [Export] private Button backButton { get; set; } = null!;

    public override void _Ready() => backButton.Pressed += () => EmitSignal(SignalName.BackPressed);
}
