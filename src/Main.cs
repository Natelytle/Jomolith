using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu;

namespace Jomolith;

[Meta(typeof(IAutoNode))]
public partial class Main : Node
{
    public override void _Notification(int what) => this.Notify(what);

    [Node("%MenuManager")]
    private IMenuManager menuManager { get; set; } = null!;

    public void OnResolved()
    {
        menuManager.QuitRequested += () => GetTree().Quit();
    }
}
