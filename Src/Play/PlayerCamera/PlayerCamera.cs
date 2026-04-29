using Chickensoft.AutoInject;
using Chickensoft.GodotNodeInterfaces;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Game.Domain;
using Jomolith.Play.PlayerCamera.State;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.PlayerCamera;

public interface IPlayerCamera : INode3D
{
    IPlayerCameraLogic CameraLogic { get; }

    Vector3 OffsetPosition { get; }
    float HorizontalRotation { get; }
    float VerticalRotation { get; }
    float SpringArmLength { get; }
    
    /// <summary>
    /// The distance the Camera3D is from the OffsetNode it is attached to. Basically, the current length of the spring arm.
    /// </summary>
    float CameraDistance { get; }
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
    
    public IPlayerCameraLogic CameraLogic { get; set; } = null!;

    public PlayerCameraLogic.CameraData CameraData { get; set; } = null!;

    public PlayerCameraLogic.CameraSettings Settings { get; set; } = null!;

    public LogicBlock<PlayerCameraLogic.CameraState>.IBinding CameraBinding { get; set; } = null!;

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
        CameraLogic = new PlayerCameraLogic();

        CameraData = new PlayerCameraLogic.CameraData
        {
            CameraLocked = false,
            DesiredZoomLength = 12.5f,
            RightClickPressed = false
        };

        Settings = new PlayerCameraLogic.CameraSettings
        {
            Sensitivity = 1.0f
        };
        
        CameraLogic.Set(this as IPlayerCamera);
        CameraLogic.Set(TowerRepo);
        CameraLogic.Set(GameRepo);
        CameraLogic.Set(CameraData);
        CameraLogic.Set(Settings);
    }

    public void OnResolved()
    {
        CameraBinding = CameraLogic.Bind();

        CameraBinding
            .Handle((in PlayerCameraLogic.Output.GlobalPositionChanged output) =>
            {
                GlobalPosition = output.GlobalPosition;
            })
            .Handle((in PlayerCameraLogic.Output.RotationChanged output) =>
            {
                SetRotation(new Vector3(output.VerticalRotation, output.HorizontalRotation, 0f));
            })
            .Handle((in PlayerCameraLogic.Output.OffsetChanged output) =>
            {
                OffsetNode.Position = output.Position;
            })
            .Handle((in PlayerCameraLogic.Output.SpringLengthChanged output) =>
            {
                SpringArm3D.SpringLength = output.Length;

                // TODO: Set character opacity to some value based on the length here
            });

    }

    public void OnReady()
    {
        SetPhysicsProcess(true);
    }

    public void OnPhysicsProcess(double delta)
    {
        CameraLogic.Input(new PlayerCameraLogic.Input.PhysicsTick(delta));
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event.IsAction("ZoomIn") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("ZoomIn");

            CameraLogic.Input(new PlayerCameraLogic.Input.ZoomedIn(zoomStrength));
        }

        if (@event.IsAction("ZoomOut") && @event.IsPressed())
        {
            float zoomStrength = Input.GetActionStrength("ZoomOut");
            
            CameraLogic.Input(new PlayerCameraLogic.Input.ZoomedOut(zoomStrength));
        }

        if (@event is InputEventMouseMotion motion)
        {
            CameraLogic.Input(new PlayerCameraLogic.Input.MouseInputOccurred(motion));
        }

        if (@event.IsAction("ToggleShiftLock") && @event.IsPressed())
        {
            CameraLogic.Input(new PlayerCameraLogic.Input.ToggleShiftLock());
        }
    }
}