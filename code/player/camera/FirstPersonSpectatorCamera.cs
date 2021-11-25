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
    public partial class FirstPersonSpectatorCamera : Sandbox.Camera, IObservationCamera
    {
        private const float SMOOTH_SPEED = 25f;

        public override void Deactivated()
        {
            base.Deactivated();

            if (Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            player.CurrentPlayer.RenderColor = Color.White;
            player.CurrentPlayer = null;
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

                Position = player.CurrentPlayer.EyePos;
                Rotation = player.CurrentPlayer.EyeRot;
            }
            else
            {
                Position = Vector3.Lerp(Position, player.CurrentPlayer.EyePos, SMOOTH_SPEED * Time.Delta);
                Rotation = Rotation.Slerp(Rotation, player.CurrentPlayer.EyeRot, SMOOTH_SPEED * Time.Delta);
            }
        }

        public void OnUpdateObservatedPlayer(TTTPlayer oldObservatedPlayer, TTTPlayer newObservatedPlayer)
        {
            if (oldObservatedPlayer != null)
            {
                oldObservatedPlayer.RenderColor = Color.White;
            }

            if (newObservatedPlayer != null)
            {
                newObservatedPlayer.RenderColor = Color.Transparent;
            }
        }
    }
}
