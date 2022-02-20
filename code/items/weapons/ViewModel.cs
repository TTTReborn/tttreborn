using System;

using Sandbox;

namespace TTTReborn.Items
{
    public partial class ViewModel : BaseViewModel
    {
        private float _walkBob = 0;

        public override void PostCameraSetup(ref CameraSetup camSetup)
        {
            base.PostCameraSetup(ref camSetup);

            AddCameraEffects(ref camSetup);
        }

        private void AddCameraEffects(ref CameraSetup camSetup)
        {
            Rotation = Local.Pawn.EyeRotation;

            float speed = Owner.Velocity.Length.LerpInverse(0, 320);
            Vector3 left = camSetup.Rotation.Left;
            Vector3 up = camSetup.Rotation.Up;

            if (Owner.GroundEntity != null)
            {
                _walkBob += Time.Delta * 25.0f * speed;
            }

            Position += up * MathF.Sin(_walkBob) * speed * -1;
            Position += left * MathF.Sin(_walkBob * 0.6f) * speed * -0.5f;
        }
    }

}
