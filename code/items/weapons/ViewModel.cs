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

using System;

using Sandbox;

namespace TTTReborn.Items
{
    partial class ViewModel : BaseViewModel
    {
        private float _walkBob = 0;

        public override void PostCameraSetup(ref CameraSetup camSetup)
        {
            base.PostCameraSetup(ref camSetup);

            AddCameraEffects(ref camSetup);
        }

        private void AddCameraEffects(ref CameraSetup camSetup)
        {
            Rotation = Local.Pawn.EyeRot;

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
