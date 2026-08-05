using Chickensoft.GodotNodeInterfaces;
using Godot;

namespace Jomolith.Gameplay.PauseMenu;

public interface IPauseMenu : IControl
{
    event PauseMenu.ResumePressedEventHandler ResumePressed;
    event PauseMenu.ExitPressedEventHandler ExitPressed;
}

public partial class PauseMenu : Control, IPauseMenu
{
    [Signal] public delegate void ResumePressedEventHandler();
    [Signal] public delegate void ExitPressedEventHandler();

    [Export] private Button resumeButton { get; set; } = null!;
    [Export] private Button exitButton { get; set; } = null!;

    public override void _Ready()
    {
        resumeButton.Pressed += () => EmitSignal(SignalName.ResumePressed);
        exitButton.Pressed += () => EmitSignal(SignalName.ExitPressed);
    }
}
