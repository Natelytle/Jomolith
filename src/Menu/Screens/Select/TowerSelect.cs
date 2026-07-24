using Chickensoft.AutoInject;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Menu.State;

namespace Jomolith.Menu.Screens.Select;

[Meta(typeof(IDependent))]
public partial class TowerSelect : Control, IScreen
{
    public override void _Notification(int what) => this.Notify(what);

    [Dependency]
    private IMenuLogic menuLogic => this.DependOn<IMenuLogic>();

    public void OnEnter()
    {
    }

    public void OnExit()
    {
    }
}
