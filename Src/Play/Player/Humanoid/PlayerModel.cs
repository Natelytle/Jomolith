using System.Collections.Generic;
using System.Linq;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.Sync.Primitives;
using Godot;
using Jomolith.Play.Player.Humanoid.State;
using Jomolith.Play.Player.Humanoid.Utils;

namespace Jomolith.Play.Player.Humanoid;

public delegate void OnHeadMovedHandler(Basis newPosition);

public delegate void OnTorsoMovedHandler(Basis newPosition);

public interface IPlayerModel : INode3D
{
    event OnHeadMovedHandler HeadMoved;
    event OnTorsoMovedHandler TorsoMoved;
}

[Meta(typeof(IAutoNode))]
public partial class PlayerModel : Node3D, IPlayerModel
{
    public override void _Notification(int what) => this.Notify(what);

    public const string ANIM_STATE_MACHINE = "parameters/StateMachine/playback";

    #region Dependencies

    [Dependency] public IPlayerLogic PlayerLogic => this.DependOn<IPlayerLogic>();

    #endregion

    #region Events

    public event OnHeadMovedHandler? HeadMoved;
    public event OnTorsoMovedHandler? TorsoMoved;

    #endregion

    #region State

    public PlayerLogic.IBinding PlayerBinding { get; set; } = null!;

    private PlayerModelMaterials Materials { get; set; } = null!;

    #endregion

    #region Nodes

    [Node("%AnimationTree")] public IAnimationTree AnimationTree { get; set; } = null!;
    public IAnimationNodeStateMachinePlayback AnimationStateMachine { get; set; } = null!;

    #endregion

    // Called when the node enters the scene tree for the first time.
    public void OnReady()
    {
        AnimationStateMachine =
            GodotInterfaces.Adapt<IAnimationNodeStateMachinePlayback>(
                (AnimationNodeStateMachinePlayback)AnimationTree.Get(
                    ANIM_STATE_MACHINE
                )
            );

        Materials = new PlayerModelMaterials(this);
    }

    public void OnResolved()
    {
        PlayerBinding = PlayerLogic.Bind();

        PlayerBinding
            .Handle((in PlayerLogic.Output.Animations.Idle _) =>
                AnimationStateMachine.Travel("Idle")
            )
            .Handle((in PlayerLogic.Output.Animations.Walk _) =>
                AnimationStateMachine.Travel("Walk")
            )
            .Handle((in PlayerLogic.Output.Animations.Jump _) =>
                AnimationStateMachine.Travel("Jump")
            )
            .Handle((in PlayerLogic.Output.Animations.Fall _) =>
                AnimationStateMachine.Travel("Fall")
            )
            .Handle((in PlayerLogic.Output.Animations.Climb _) =>
                AnimationStateMachine.Travel("Climb")
            )
            .Handle((in PlayerLogic.Output.Animations.Enabled _) =>
                AnimationTree.Set("parameters/Transitions/transition_request", "Enabled")
            )
            .Handle((in PlayerLogic.Output.Animations.Disabled _) =>
                AnimationTree.Set("parameters/Transitions/transition_request", "Disabled")
            )
            .Handle((in PlayerLogic.Output.FloorVelocityChanged output) =>
                AnimationTree.Set("parameters/StateMachine/Walk/Speed/scale", (float)(output.Velocity.Length() / 16.0))
            )
            .Handle((in PlayerLogic.Output.VerticalVelocityChanged output) =>
                AnimationTree.Set("parameters/StateMachine/Climb/Speed/scale", (float)(output.Velocity.Length() / 12.0))
            )
            .Handle((in PlayerLogic.Output.Visual.SetTransparency output) =>
                Materials.SetOpacity(output.Alpha)
            );
    }
}
