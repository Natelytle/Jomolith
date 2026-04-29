using System;
using Chickensoft.Introspection;
using Chickensoft.LogicBlocks;
using Godot;
using Jomolith.Play.Tower.Domain;

namespace Jomolith.Play.PlayerCamera.State;

public partial class PlayerCameraLogic
{
    [Meta]
    public abstract partial record CameraState : StateLogic<CameraState>, 
        IGet<Input.PhysicsTick>,
        IGet<Input.MouseInputOccurred>,
        IGet<Input.RightClickPressed>,
        IGet<Input.RightClickReleased>,
        IGet<Input.ZoomedIn>,
        IGet<Input.ZoomedOut>
    {
        private const float first_person_threshold = 1.0f;
        private static readonly Vector2 MousePxToUnits = new Vector2(0.002f * float.Pi, 0.0015f * float.Pi);

        public Transition On(in Input.PhysicsTick input)
        {
            IPlayerCamera camera = Get<IPlayerCamera>();
            CameraData data = Get<CameraData>();
            ITowerRepo towerRepo = Get<ITowerRepo>();
            
            // Set the focus position to the position of the player.
            Output(new Output.GlobalPositionChanged(towerRepo.PlayerGlobalPosition.Value));
            
            // Spring arm should never be longer than the desired spring arm length.
            float newSpringArmLength = Math.Min(camera.SpringArmLength, camera.CameraDistance);
            
            // Lerp it to the desired position from where it currently is.
            float lerpAmount = 1 - Mathf.Pow(0.5f, (float)input.Delta * 30);
            newSpringArmLength = Mathf.Lerp(newSpringArmLength, data.DesiredZoomLength, lerpAmount);

            Output(new Output.SpringLengthChanged(newSpringArmLength));
            
            GD.Print($"[Frame {Engine.GetPhysicsFrames()}] Camera setting basis");
            towerRepo.SetCameraBasis(camera.Basis);

            return ToSelf();
        }

        public Transition On(in Input.MouseInputOccurred input)
        {
            CameraData data = Get<CameraData>();

            if (!data.ShouldPan)
                return ToSelf();

            IPlayerCamera camera = Get<IPlayerCamera>();
            CameraSettings settings = Get<CameraSettings>();

            Vector2 moveVector = input.Motion.Relative * MousePxToUnits * settings.Sensitivity;

            float newHorizontalRotation = camera.HorizontalRotation - moveVector.X;
            float newVerticalRotation = camera.VerticalRotation - moveVector.Y;

            // Clamp to 80 degrees.
            newVerticalRotation = Math.Clamp(newVerticalRotation, float.DegreesToRadians(-80), float.DegreesToRadians(80));

            Output(new Output.RotationChanged(newHorizontalRotation, newVerticalRotation));

            return ToSelf();
        }

        public Transition On(in Input.RightClickPressed _)
        {
            Get<CameraData>().RightClickPressed = true;

            return ToSelf();
        }

        public Transition On(in Input.RightClickReleased _)
        {
            Get<CameraData>().RightClickPressed = false;

            return ToSelf();
        }

        public Transition On(in Input.ZoomedIn input)
        {
            var data = Get<CameraData>();

            float currDistance = data.DesiredZoomLength;
            float newDistance = currDistance - (1 + currDistance * 0.5f) * input.ZoomStrength;

            if (newDistance < first_person_threshold)
            {
                Input(new Input.FirstPersonEntered());
                newDistance = 0f;
            }

            data.DesiredZoomLength = newDistance;

            return ToSelf();
        }

        public Transition On(in Input.ZoomedOut input)
        {
            var data = Get<CameraData>();

            float currDistance = data.DesiredZoomLength;
            float newDistance = currDistance + (1 + currDistance * 0.5f) * input.ZoomStrength;
            
            Input(new Input.FirstPersonExited());

            data.DesiredZoomLength = newDistance;

            return ToSelf();
        }
    }
}