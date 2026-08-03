using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Gameplay.Player.Humanoid.State;
using Jomolith.Gameplay.Player.Humanoid.Utils;

namespace Jomolith.Gameplay.Player.Humanoid;

public delegate void OnHeadMovedHandler(Basis newPosition);

public delegate void OnTorsoMovedHandler(Basis newPosition);

public interface IPlayerModel : INode3D
{
    event PlayerModel.HeadMovedEventHandler? HeadMoved;
    event PlayerModel.TorsoMovedEventHandler? TorsoMoved;
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

    [Signal]
    public delegate void HeadMovedEventHandler(Transform3D newTransform);

    [Signal]
    public delegate void TorsoMovedEventHandler(Transform3D newTransform);

    #endregion

    #region State

    private PlayerModelMaterials materials { get; set; } = null!;

    #endregion

    #region Nodes

    [Node("%AnimationTree")] public IAnimationTree AnimationTree { get; set; } = null!;
    public IAnimationNodeStateMachinePlayback AnimationStateMachine { get; set; } = null!;

    [Node("Avatar/Skeleton3D/Head_2")] public IBoneAttachment3D Head { get; set; } = null!;
    [Node("Avatar/Skeleton3D/Torso_2")] public IBoneAttachment3D Torso { get; set; } = null!;

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

        materials = new PlayerModelMaterials(this);

        SetPhysicsProcess(true);
    }

    public void OnResolved()
    {
        using var binding = PlayerLogic.Bind();

        binding
            .OnOutput((in PlayerLogic.Outputs.Animations.Idle _) =>
                AnimationStateMachine.Travel("Idle")
            )
            .OnOutput((in PlayerLogic.Outputs.Animations.Walk _) =>
                AnimationStateMachine.Travel("Walk")
            )
            .OnOutput((in PlayerLogic.Outputs.Animations.Jump _) =>
                AnimationStateMachine.Travel("Jump")
            )
            .OnOutput((in PlayerLogic.Outputs.Animations.Fall _) =>
                AnimationStateMachine.Travel("Fall")
            )
            .OnOutput((in PlayerLogic.Outputs.Animations.Climb _) =>
                AnimationStateMachine.Travel("Climb")
            )
            .OnOutput((in PlayerLogic.Outputs.Animations.Enabled _) =>
                AnimationTree.Set("parameters/Transitions/transition_request", "Enabled")
            )
            .OnOutput((in PlayerLogic.Outputs.Animations.Disabled _) =>
                AnimationTree.Set("parameters/Transitions/transition_request", "Disabled")
            )
            .OnOutput((in PlayerLogic.Outputs.FloorVelocityChanged output) =>
                AnimationTree.Set("parameters/StateMachine/Walk/Speed/scale", (float)(output.Velocity.Length() / 16.0))
            )
            .OnOutput((in PlayerLogic.Outputs.VerticalVelocityChanged output) =>
                AnimationTree.Set("parameters/StateMachine/Climb/Speed/scale", (float)(output.Velocity / 12.0))
            )
            .OnOutput((in PlayerLogic.Outputs.Visual.SetTransparency output) =>
                materials.SetOpacity(output.Alpha)
            );
    }

    public void OnPhysicsProcess(double delta)
    {
        // The bone attachments are at the bottom of the model, so we need to add the difference manually.
        const float head_transform_offset = 4.5f;
        const float torso_transform_offset = 3.0f;

        EmitSignal(SignalName.HeadMoved, Head.GlobalTransform with { Origin = Head.GlobalTransform.Origin - Head.GlobalTransform.Basis.Z * head_transform_offset });
        EmitSignal(SignalName.TorsoMoved, Torso.GlobalTransform with { Origin = Torso.GlobalTransform.Origin - Torso.GlobalTransform.Basis.Z * torso_transform_offset });
    }
}
