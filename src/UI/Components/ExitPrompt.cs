using Godot;

namespace Jomolith.UI.Components;

public partial class ExitPrompt : Control
{
    [Signal] public delegate void ExitConfirmedEventHandler();
    [Signal] public delegate void ExitCancelledEventHandler();

    [Export] private Button confirmButton { get; set; } = null!;
    [Export] private Button backButton { get; set; } = null!;

    public override void _Ready()
    {
        confirmButton.Pressed += () => EmitSignal(SignalName.ExitConfirmed);
        backButton.Pressed += () => EmitSignal(SignalName.ExitCancelled);
    }
}
