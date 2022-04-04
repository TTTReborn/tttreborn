using Sandbox;

namespace TTTReborn.Camera
{
    public class RagdollSpectateCamera : CameraMode, IObservationCamera
    {
        private Vector3 FocusPoint;

        public override void Activated()
        {
            base.Activated();

            FocusPoint = CurrentView.Position - GetViewOffset();
        }

        public override void Update()
        {
            FocusPoint = Vector3.Lerp(FocusPoint, GetSpectatePoint(), Time.Delta * 5.0f);

            Position = FocusPoint + GetViewOffset();
            Rotation = Input.Rotation;

            Viewer = null;
        }

        public virtual Vector3 GetSpectatePoint()
        {
            if (Local.Pawn is Player player && player.Corpse.IsValid())
            {
                return player.Corpse.PhysicsGroup.MassCenter;
            }

            return Local.Pawn.Position;
        }

        public virtual Vector3 GetViewOffset() => Input.Rotation.Forward * -130 + Vector3.Up * 20;
    }
}
