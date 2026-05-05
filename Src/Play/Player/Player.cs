using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Play.Player.Camera;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Humanoid;

namespace Jomolith.Play.Player;

public interface IPlayer : INode3D;

[Meta(typeof(IAutoNode))]
public partial class Player : Node3D, IPlayer, IProvide<IPlayerRepo>
{
    public override void _Notification(int what) => this.Notify(what);

    #region Provisions

    IPlayerRepo IProvide<IPlayerRepo>.Value() => PlayerRepo;

    #endregion

    #region State

    public IPlayerRepo PlayerRepo { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public IHumanoid Humanoid { get; set; } = null!;

    [Node] public ICamera PlayerCamera { get; set; } = null!;

    #endregion

    public void Setup()
    {
        PlayerRepo = new PlayerRepo();
    }

    public void OnResolved()
    {
        this.Provide();
    }

    public void OnReady() => SetPhysicsProcess(true);

    public void OnPhysicsProcess(double delta)
    {
        // Tick the camera before the humanoid. This gives the humanoid
        // an up-to-date camera rotation that it can use for shift lock.
        PlayerCamera.PhysicsTick(delta);
        Humanoid.PhysicsTick(delta);
    }
}
