using Sandbox;

namespace TTTReborn.Player.Camera
{
    public partial class ThirdPersonSpectateCamera : Sandbox.Camera, IObservationCamera
    {
        private Vector3 DefaultPosition { get; set; }

        private const float LERP_MODE = 0;
        private const int FIELD_OF_VIEW_OVERRIDE = 70;
        private const int CAMERA_DISTANCE = 120;

        private Rotation _targetRot;
        private Vector3 _targetPos;
        private Angles _lookAngles;

        public override void Activated()
        {
            base.Activated();

            Rotation = CurrentView.Rotation;

            FieldOfView = FIELD_OF_VIEW_OVERRIDE;
        }

        public override void Update()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (!player.IsSpectatingPlayer || Input.Pressed(InputButton.Attack1))
            {
                player.UpdateObservatedPlayer();
            }

            _targetRot = Rotation.From(_lookAngles);
            Rotation = Rotation.Slerp(Rotation, _targetRot, 10 * RealTime.Delta * (1 - LERP_MODE));

            _targetPos = GetSpectatePoint() + Rotation.Forward * -CAMERA_DISTANCE;
            Position = _targetPos;
        }

        private Vector3 GetSpectatePoint()
        {
            if (Local.Pawn is not TTTPlayer player || !player.IsSpectatingPlayer)
            {
                return DefaultPosition;
            }

            return player.CurrentPlayer.EyePos;
        }

        public override void BuildInput(InputBuilder input)
        {
            _lookAngles += input.AnalogLook * (FIELD_OF_VIEW_OVERRIDE / 80.0f);
            _lookAngles.roll = 0;

            base.BuildInput(input);
        }

        public override void Deactivated()
        {
            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            player.CurrentPlayer = null;
        }
    }
}
