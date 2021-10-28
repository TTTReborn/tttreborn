using Sandbox;

namespace TTTReborn.Player.Camera
{
    public partial class FreeSpectateCamera : Sandbox.Camera, IObservationCamera
    {
        private Angles _lookAngles;
        private Vector3 _moveInput;

        private Vector3 _targetPos;
        private Rotation _targetRot;

        private float _moveSpeed;

        private const float LERP_MODE = 0;
        private const float FIELD_OF_VIEW_OVERRIDE = 70.0f;
        private const float DEFAULT_FIELD_OF_VIEW = 80.0f;

        public override void Activated()
        {
            base.Activated();

            FieldOfView = FIELD_OF_VIEW_OVERRIDE;

            _targetPos = CurrentView.Position;
            _targetRot = CurrentView.Rotation;

            Position = _targetPos;
            Rotation = _targetRot;
            _lookAngles = Rotation.Angles();
        }

        public override void Update()
        {
            if (Local.Client == null)
            {
                return;
            }

            Vector3 mv = _moveInput.Normal * 300 * RealTime.Delta * Rotation * _moveSpeed;

            _targetRot = Rotation.From(_lookAngles);
            _targetPos += mv;

            Position = Vector3.Lerp(Position, _targetPos, 10 * RealTime.Delta * (1 - LERP_MODE));
            Rotation = Rotation.Slerp(Rotation, _targetRot, 10 * RealTime.Delta * (1 - LERP_MODE));
        }

        public override void BuildInput(InputBuilder input)
        {
            _moveInput = input.AnalogMove;

            _moveSpeed = 1;

            if (input.Down(InputButton.Run))
            {
                _moveSpeed = 5;
            }

            if (input.Down(InputButton.Duck))
            {
                _moveSpeed = 0.2f;
            }

            _lookAngles += input.AnalogLook * (FIELD_OF_VIEW_OVERRIDE / DEFAULT_FIELD_OF_VIEW);
            _lookAngles.roll = 0;

            base.BuildInput(input);
        }
    }
}
