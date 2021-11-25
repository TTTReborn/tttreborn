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
    public class RagdollSpectateCamera : Sandbox.Camera, IObservationCamera
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
            if (Local.Pawn is TTTPlayer player && player.Corpse.IsValid())
            {
                return player.Corpse.PhysicsGroup.MassCenter;
            }

            return Local.Pawn.Position;
        }

        public virtual Vector3 GetViewOffset()
        {
            return Input.Rotation.Forward * -130 + Vector3.Up * 20;
        }
    }
}
