using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Play.PlayerCamera.LockModeState;
using Jomolith.Play.PlayerCamera.PerspectiveState;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.PlayerCamera;

public interface IPlayerCamera : INode3D
{
    ILockModeLogic LockModeLogic { get; }
    IPerspectiveLogic PerspectiveLogic { get; }
}

[Meta(typeof(IAutoNode))]
public partial class PlayerCamera : Node3D, IPlayerCamera
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies
    
    [Dependency]
    public ITowerRepo TowerRepo => this.DependOn<ITowerRepo>();
    
    [Dependency]
    public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    #endregion 

    #region State

    public ILockModeLogic LockModeLogic { get; set; } = null!;
    public LogicBlock<LockModeLogic.LockModeState>.IBinding LockModeBinding { get; set; } = null!;

    public IPerspectiveLogic PerspectiveLogic { get; set; } = null!;
    public LogicBlock<PerspectiveLogic.PerspectiveState>.IBinding PerspectiveBinding { get; set; } = null!;

    #endregion
    
    public void Setup()
    {
        LockModeLogic = new LockModeLogic();
    }

    public void OnResolved()
    {
        LockModeBinding = LockModeLogic.Bind();

        LockModeBinding
            .Handle((in LockModeLogic.Output.ShiftLockEntered _) =>
            {

            })
            .Handle((in LockModeLogic.Output.ShiftLockExited _) =>
            {

            });

        PerspectiveBinding = PerspectiveLogic.Bind();

        PerspectiveBinding
            .Handle((in PerspectiveLogic.Output.FirstPersonEntered _) =>
            {

            })
            .Handle((in PerspectiveLogic.Output.FirstPersonExited _) =>
            {
                
            });

        LockModeLogic.Start();
        PerspectiveLogic.Start();
    }

    public void OnReady()
    {
        SetPhysicsProcess(true);
    }

    public void OnPhysicsProcess(double delta)
    {
    }
}