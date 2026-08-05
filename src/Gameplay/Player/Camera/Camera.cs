using System;
using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Godot;
using Jomolith.Gameplay.Domain;
using Jomolith.Gameplay.Player.Domain;
using Jomolith.Gameplay.Player.Camera.State;
using Jomolith.Gameplay.Player.Camera.State.States;
using Jomolith.Settings.Domain.Models;

namespace Jomolith.Gameplay.Player.Camera;

public interface ICamera : INode3D
{
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

    [Dependency] private IPlayerRepo playerRepo => this.DependOn<IPlayerRepo>();

    [Dependency] private IGameplayRepo gameplayRepo => this.DependOn<IGameplayRepo>();

    [Dependency] private GameplaySettings gameplaySettings => this.DependOn<GameplaySettings>();

    #endregion

    #region State

    private ICameraLogic cameraLogic { get; set; } = null!;

    private CameraLogic.CameraData cameraData { get; set; } = null!;

    #endregion

    #region Nodes

    [Node("%OffsetNode")] private Node3D offsetNode { get; set; } = null!;

    [Node("%SpringArm3D")] private ISpringArm3D springArm3D { get; set; } = null!;

    [Node("%Camera3D")] private ICamera3D camera3D { get; set; } = null!;

    #endregion

    #region Computed

    public Vector3 OffsetPosition => offsetNode.Position;

    public float VerticalRotation => Rotation.X;

    public float HorizontalRotation => Rotation.Y;

    public float SpringArmLength => springArm3D.SpringLength;

    public float CameraDistance => (camera3D.GlobalPosition - offsetNode.GlobalPosition).Length();

    #endregion

    public void Setup()
    {
        cameraLogic = new CameraLogic();

        cameraData = new CameraLogic.CameraData
        {
            CameraLockedState = false,
            DesiredZoomLength = 12.5f,
            CameraLockedRightClick = false
        };

        cameraLogic.Set(this as ICamera);
        cameraLogic.Set(playerRepo);
        cameraLogic.Set(cameraData);
        cameraLogic.Set(gameplaySettings);
    }

    public void OnResolved()
    {
        cameraLogic.Bind()
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
                offsetNode.Position = output.Position;
            })
            .OnOutput((in CameraLogic.Outputs.SpringLengthChanged output) =>
            {
                springArm3D.SpringLength = output.Length;

                playerRepo.SetAvatarOpacity(MathF.Pow(Math.Clamp(output.Length / 2, 0, 1), 2));
            })
            .OnOutput((in CameraLogic.Outputs.SetCameraLocked output) =>
            {
                cameraData.CameraLockedState = output.Value;

                gameplayRepo.SetIsMouseCaptured(cameraData.CameraLocked);
            })
            .OnOutput((in CameraLogic.Outputs.SetRightClickPressed output) =>
            {
                cameraData.CameraLockedRightClick = output.Value;

                gameplayRepo.SetIsMouseCaptured(cameraData.CameraLocked);
            })
            .OnOutput((in CameraLogic.Outputs.SetPlayerLocked output) =>
            {
                playerRepo.SetIsPlayerRotationLocked(output.Value);
            });

        cameraLogic.Start<CameraState.Unlocked>();
    }

    public void PhysicsTick(double delta)
    {
        cameraLogic.Input(new CameraLogic.Inputs.PhysicsTick(delta));
    }

    public void PostPhysicsTick()
    {
        cameraLogic.Input(new CameraLogic.Inputs.PostPhysicsTick());
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsAction("zoom_in") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("zoom_in");

            cameraLogic.Input(new CameraLogic.Inputs.ZoomedIn(zoomStrength));
        }

        if (@event.IsAction("zoom_out") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("zoom_out");

            cameraLogic.Input(new CameraLogic.Inputs.ZoomedOut(zoomStrength));
        }

        if (@event is InputEventMouseMotion motion)
        {
            cameraLogic.Input(new CameraLogic.Inputs.MouseInputOccurred(motion));
        }

        if (@event.IsAction("shift_lock") && @event.IsPressed())
        {
            cameraLogic.Input(new CameraLogic.Inputs.ToggleShiftLock());
        }

        if (@event is InputEventMouseButton button
            && button.ButtonIndex is MouseButton.Right)
        {
            if (button.IsPressed() && !cameraData.CameraLockedRightClick)
                cameraLogic.Input(new CameraLogic.Inputs.RightClickPressed());
            if (button.IsReleased() && cameraData.CameraLockedRightClick)
                cameraLogic.Input(new CameraLogic.Inputs.RightClickReleased());
        }
    }
}
