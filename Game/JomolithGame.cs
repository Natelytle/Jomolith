using Godot;

namespace Jomolith.Game;

public partial class JomolithGame : Control
{
  public Button TestButton { get; private set; } = default!;
  public int ButtonPresses { get; private set; }

  public override void _Ready()
    => TestButton = GetNode<Button>("%TestButton");

  public void OnTestButtonPressed() => ButtonPresses++;
}
