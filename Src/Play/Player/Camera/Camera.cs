using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Play.Gameplay.Domain;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Camera.State;

namespace Jomolith.Play.Player.Camera;

public interface ICamera : INode3D
{
    ICameraLogic CameraLogic { get; }

    Vector3 OffsetPosition { get; }
    float HorizontalRotation { get; }
    float VerticalRotation { get; }
    float SpringArmLength { get; }

    /// <summary>
    /// The distance the Camera3D is from the OffsetNode it is attached to. Basically, the current length of the spring arm.
    /// </summary>
    float CameraDistance { get; }

    void PhysicsTick(double delta);
}

[Meta(typeof(IAutoNode))]
public partial class Camera : Node3D, ICamera
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency]
    public IPlayerRepo PlayerRepo => this.DependOn<IPlayerRepo>();

    [Dependency]
    public IGameplayRepo GameplayRepo => this.DependOn<IGameplayRepo>();

    [Dependency]
    public IGameRepo GameRepo => this.DependOn<IGameRepo>();

    #endregion

    #region State

    public ICameraLogic CameraLogic { get; set; } = null!;

    public CameraLogic.CameraData CameraData { get; set; } = null!;

    public CameraLogic.CameraSettings Settings { get; set; } = null!;

    public LogicBlock<CameraLogic.CameraState>.IBinding CameraBinding { get; set; } = null!;

    #endregion

    #region Nodes

    [Node] public Node3D OffsetNode { get; set; } = null!;

    [Node] public ISpringArm3D SpringArm3D { get; set; } = null!;

    [Node] public ICamera3D Camera3D { get; set; } = null!;

    #endregion

    #region Computed

    public Vector3 OffsetPosition => OffsetNode.Position;

    public float VerticalRotation => Rotation.X;

    public float HorizontalRotation => Rotation.Y;

    public float SpringArmLength => SpringArm3D.SpringLength;

    public float CameraDistance => (Camera3D.GlobalPosition - OffsetNode.GlobalPosition).Length();

    #endregion

    public void Setup()
    {
        CameraLogic = new CameraLogic();

        CameraData = new CameraLogic.CameraData
        {
            CameraLockedState = false,
            DesiredZoomLength = 12.5f,
            CameraLockedRightClick = false
        };

        Settings = new CameraLogic.CameraSettings
        {
            Sensitivity = 1.0f
        };

        CameraLogic.Set(this as ICamera);
        CameraLogic.Set(PlayerRepo);
        CameraLogic.Set(GameRepo);
        CameraLogic.Set(CameraData);
        CameraLogic.Set(Settings);
    }

    public void OnResolved()
    {
        CameraBinding = CameraLogic.Bind();

        CameraBinding
            .Handle((in CameraLogic.Output.GlobalPositionChanged output) =>
            {
                GlobalPosition = output.GlobalPosition;
            })
            .Handle((in CameraLogic.Output.RotationChanged output) =>
            {
                SetRotation(new Vector3(output.VerticalRotation, output.HorizontalRotation, 0f));
            })
            .Handle((in CameraLogic.Output.OffsetChanged output) =>
            {
                OffsetNode.Position = output.Position;
            })
            .Handle((in CameraLogic.Output.SpringLengthChanged output) =>
            {
                SpringArm3D.SpringLength = output.Length;

                // TODO: Set character opacity to some value based on the length here
            })
            .Handle((in CameraLogic.Output.SetCameraLocked output) =>
            {
                CameraData.CameraLockedState = output.Value;

                GameplayRepo.SetIsMouseCaptured(CameraData.CameraLocked);
            })
            .Handle((in CameraLogic.Output.SetRightClickPressed output) =>
            {
                CameraData.CameraLockedRightClick = output.Value;

                GameplayRepo.SetIsMouseCaptured(CameraData.CameraLocked);
            })
            .Handle((in CameraLogic.Output.SetPlayerLocked output) =>
            {
                PlayerRepo.SetIsPlayerRotationLocked(output.Value);
            });
    }

    public void PhysicsTick(double delta)
    {
        CameraLogic.Input(new CameraLogic.Input.PhysicsTick(delta));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("ZoomIn") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("ZoomIn");

            CameraLogic.Input(new CameraLogic.Input.ZoomedIn(zoomStrength));
        }

        if (@event.IsAction("ZoomOut") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("ZoomOut");

            CameraLogic.Input(new CameraLogic.Input.ZoomedOut(zoomStrength));
        }

        if (@event is InputEventMouseMotion motion)
        {
            CameraLogic.Input(new CameraLogic.Input.MouseInputOccurred(motion));
        }

        if (@event.IsAction("ToggleShiftLock") && @event.IsPressed())
        {
            CameraLogic.Input(new CameraLogic.Input.ToggleShiftLock());
        }

        if (@event is InputEventMouseButton button
            && button.ButtonIndex is MouseButton.Right)
        {
            if (button.IsPressed() && !CameraData.CameraLockedRightClick)
                CameraLogic.Input(new CameraLogic.Input.RightClickPressed());
            if (button.IsReleased() && CameraData.CameraLockedRightClick)
                CameraLogic.Input(new CameraLogic.Input.RightClickReleased());
        }
    }
}
