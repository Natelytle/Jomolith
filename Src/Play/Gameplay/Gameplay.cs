using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Play.Gameplay.Domain;

namespace Jomolith.Play.Gameplay;

public interface IGameplay : INode3D, IProvide<IGameplayRepo>
{
}

[Meta(typeof(IAutoNode))]
public partial class Gameplay : Node3D, IGameplay
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] 
    public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    #endregion

    #region Provisions

    IGameplayRepo IProvide<IGameplayRepo>.Value() => GameplayRepo;

    #endregion

    #region State

    public IGameplayRepo GameplayRepo { get; set; } = null!;

    #endregion

    public void Setup()
    {
        GameplayRepo = new GameplayRepo();
    }

    public void OnResolved()
    {
        this.Provide();
    }
}