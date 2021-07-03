using System;
using Sandbox;

namespace TTTReborn.Player.Camera
{
    public class SpectateCamera : Sandbox.Camera
    {
        private Angles _lookAngles;
        private Vector3 _moveInput;

        private Vector3 _targetPos;
        private Rotation _targetRot;

        private float _moveSpeed;
        private float _fovOverride;

        private const float LERP_MODE = 0;

        public override void Activated()
        {
            base.Activated();

            _targetPos = CurrentView.Position;
            _targetRot = CurrentView.Rotation;

            Pos = _targetPos;
            Rot = _targetRot;
            _lookAngles = Rot.Angles();
            _fovOverride = 80;

            DoFPoint = 0.0f;
            DoFBlurSize = 0.0f;
        }

        public override void Update()
        {
            var player = Local.Client;
            if (player == null)
            {
                return;
            }

            FieldOfView = _fovOverride;

            var mv = _moveInput.Normal * 300 * RealTime.Delta * Rot * _moveSpeed;

            _targetRot = Rotation.From(_lookAngles);
            _targetPos += mv;

            Pos = Vector3.Lerp(Pos, _targetPos, 10 * RealTime.Delta * (1 - LERP_MODE));
            Rot = Rotation.Slerp(Rot, _targetRot, 10 * RealTime.Delta * (1 - LERP_MODE));
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

            _lookAngles += input.AnalogLook * (_fovOverride / 80.0f);
            _lookAngles.roll = 0;

            base.BuildInput(input);
        }
    }
}
