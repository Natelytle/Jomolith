using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Menu.Components;

public interface IExitPrompt : IControl
{
    event ExitPrompt.ExitConfirmedEventHandler ExitConfirmed;
    event ExitPrompt.ExitCancelledEventHandler ExitCancelled;
};

public partial class ExitPrompt : Control, IExitPrompt
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
