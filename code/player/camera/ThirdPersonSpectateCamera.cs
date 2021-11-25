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
    public partial class ThirdPersonSpectateCamera : Sandbox.Camera, IObservationCamera
    {
        private Vector3 DefaultPosition { get; set; }

        private const float LERP_MODE = 0;
        private const int CAMERA_DISTANCE = 120;

        private Rotation _targetRot;
        private Vector3 _targetPos;
        private Angles _lookAngles;

        public override void Activated()
        {
            base.Activated();

            Rotation = CurrentView.Rotation;
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
            _lookAngles += input.AnalogLook;
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
