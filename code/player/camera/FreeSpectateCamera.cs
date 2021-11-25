// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

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

        public override void Activated()
        {
            base.Activated();

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

            _lookAngles += input.AnalogLook;
            _lookAngles.roll = 0;

            base.BuildInput(input);
        }
    }
}
