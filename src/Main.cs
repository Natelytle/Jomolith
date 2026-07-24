using Godot;
using Jomolith.UI;

namespace Jomolith;

public partial class Main : Node
{
    [Export] private UIManager uiManager = null!;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        uiManager.QuitRequested += () => GetTree().Quit();
    }
}
