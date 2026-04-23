using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.Tower;

public interface ITower : INode3D, IProvide<ITowerRepo>
{
}

[Meta(typeof(IAutoNode))]
public partial class Tower : Node3D, ITower
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] 
    public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    #endregion

    #region Provisions

    ITowerRepo IProvide<ITowerRepo>.Value() => TowerRepo;

    #endregion

    #region State

    public ITowerRepo TowerRepo { get; set; } = null!;

    #endregion

    public void Setup()
    {
        TowerRepo = new TowerRepo();
    }

    public void OnResolved()
    {
        this.Provide();
    }
}