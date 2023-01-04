using Sandbox;

namespace TTTReborn.Camera
{
    public class RagdollSpectateCamera : CameraMode, IObservationCamera
    {
        private Vector3 FocusPoint;

        public override void Activated()
        {
            base.Activated();

            FocusPoint = Game.LocalPawn.Position - GetViewOffset();
        }

        public override void Update()
        {
            FocusPoint = Vector3.Lerp(FocusPoint, GetSpectatePoint(), Time.Delta * 5.0f);

            Position = FocusPoint + GetViewOffset();
            Rotation = Input.AnalogLook.ToRotation();

            Viewer = null;
        }

        public virtual Vector3 GetSpectatePoint()
        {
            if (Game.LocalPawn is Player player && player.Corpse.IsValid())
            {
                return player.Corpse.PhysicsGroup.MassCenter;
            }

            return Game.LocalPawn.Position;
        }

        public virtual Vector3 GetViewOffset() => Input.AnalogLook.Forward * -130 + Vector3.Up * 20;
    }
}
