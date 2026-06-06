using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.App.Domain;
using Jomolith.Play.Gameplay.Domain;
using Jomolith.Play.Player.Domain;
using Jomolith.Play.Player.Camera.State;
using Jomolith.Play.Player.Camera.State.States;

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

    void PostPhysicsTick();
}

[Meta(typeof(IAutoNode))]
public partial class Camera : Node3D, ICamera
{
    public override void _Notification(int what) => this.Notify(what);

    #region Dependencies

    [Dependency] public IPlayerRepo PlayerRepo => this.DependOn<IPlayerRepo>();

    [Dependency] public IGameplayRepo GameplayRepo => this.DependOn<IGameplayRepo>();

    #endregion

    #region State

    public ICameraLogic CameraLogic { get; set; } = null!;

    public CameraLogic.CameraData CameraData { get; set; } = null!;

    public CameraLogic.CameraSettings Settings { get; set; } = null!;

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
            Sensitivity = 0.24f
        };

        CameraLogic.Set(this as ICamera);
        CameraLogic.Set(PlayerRepo);
        CameraLogic.Set(CameraData);
        CameraLogic.Set(Settings);
    }

    public void OnResolved()
    {
        using var binding = CameraLogic.Bind();

        binding
            .OnOutput((in CameraLogic.Outputs.GlobalPositionChanged output) =>
            {
                GlobalPosition = output.GlobalPosition;
            })
            .OnOutput((in CameraLogic.Outputs.RotationChanged output) =>
            {
                SetRotation(new Vector3(output.VerticalRotation, output.HorizontalRotation, 0f));
            })
            .OnOutput((in CameraLogic.Outputs.OffsetChanged output) =>
            {
                OffsetNode.Position = output.Position;
            })
            .OnOutput((in CameraLogic.Outputs.SpringLengthChanged output) =>
            {
                SpringArm3D.SpringLength = output.Length;

                PlayerRepo.SetAvatarOpacity(MathF.Pow(Math.Clamp(output.Length / 2, 0, 1), 2));
            })
            .OnOutput((in CameraLogic.Outputs.SetCameraLocked output) =>
            {
                CameraData.CameraLockedState = output.Value;

                GameplayRepo.SetIsMouseCaptured(CameraData.CameraLocked);
            })
            .OnOutput((in CameraLogic.Outputs.SetRightClickPressed output) =>
            {
                CameraData.CameraLockedRightClick = output.Value;

                GameplayRepo.SetIsMouseCaptured(CameraData.CameraLocked);
            })
            .OnOutput((in CameraLogic.Outputs.SetPlayerLocked output) =>
            {
                PlayerRepo.SetIsPlayerRotationLocked(output.Value);
            });

        CameraLogic.Start<CameraState.Unlocked>();
    }

    public void PhysicsTick(double delta)
    {
        CameraLogic.Input(new CameraLogic.Inputs.PhysicsTick(delta));
    }

    public void PostPhysicsTick()
    {
        CameraLogic.Input(new CameraLogic.Inputs.PostPhysicsTick());
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("ZoomIn") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("ZoomIn");

            CameraLogic.Input(new CameraLogic.Inputs.ZoomedIn(zoomStrength));
        }

        if (@event.IsAction("ZoomOut") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("ZoomOut");

            CameraLogic.Input(new CameraLogic.Inputs.ZoomedOut(zoomStrength));
        }

        if (@event is InputEventMouseMotion motion)
        {
            CameraLogic.Input(new CameraLogic.Inputs.MouseInputOccurred(motion));
        }

        if (@event.IsAction("ToggleShiftLock") && @event.IsPressed())
        {
            CameraLogic.Input(new CameraLogic.Inputs.ToggleShiftLock());
        }

        if (@event is InputEventMouseButton button
            && button.ButtonIndex is MouseButton.Right)
        {
            if (button.IsPressed() && !CameraData.CameraLockedRightClick)
                CameraLogic.Input(new CameraLogic.Inputs.RightClickPressed());
            if (button.IsReleased() && CameraData.CameraLockedRightClick)
                CameraLogic.Input(new CameraLogic.Inputs.RightClickReleased());
        }
    }
}
