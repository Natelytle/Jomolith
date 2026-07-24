using Godot;

namespace Jomolith.UI.Components;

public partial class Footer : Control
{
    [Signal] public delegate void BackPressedEventHandler();

    [Export] private Button backButton { get; set; } = null!;

    public override void _Ready() => backButton.Pressed += () => EmitSignal(SignalName.BackPressed);
}
