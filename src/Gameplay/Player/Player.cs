using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Gameplay.Player.Camera;
using Jomolith.Gameplay.Player.Domain;
using Jomolith.Gameplay.Player.Humanoid;

namespace Jomolith.Gameplay.Player;

public interface IPlayer : INode3D;

[Meta(typeof(IAutoNode))]
public partial class Player : Node3D, IPlayer, IProvide<IPlayerRepo>
{
    public override void _Notification(int what) => this.Notify(what);

    #region Provisions

    IPlayerRepo IProvide<IPlayerRepo>.Value() => playerRepo;

    #endregion

    #region State

    private IPlayerRepo playerRepo { get; set; } = null!;

    #endregion

    #region Nodes

    [Node("%Humanoid")] private IHumanoid humanoid { get; set; } = null!;

    [Node("%PlayerCamera")] private ICamera playerCamera { get; set; } = null!;

    #endregion

    public void Setup()
    {
        playerRepo = new PlayerRepo();
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
        playerCamera.PhysicsTick(delta);
        humanoid.PhysicsTick(delta);

        // Tick the camera again to recenter it to the new humanoid position.
        playerCamera.PostPhysicsTick();
    }
}
