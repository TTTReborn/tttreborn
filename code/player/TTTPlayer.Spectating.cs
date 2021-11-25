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

using System.Collections.Generic;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Player.Camera;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        private TTTPlayer _spectatingPlayer;
        public TTTPlayer CurrentPlayer
        {
            get => _spectatingPlayer ?? this;
            set
            {
                _spectatingPlayer = value == this ? null : value;

                Event.Run(TTTEvent.Player.Spectating.Change, this);
            }
        }

        public bool IsSpectatingPlayer
        {
            get => _spectatingPlayer != null;
        }

        public bool IsSpectator
        {
            get => (Camera is IObservationCamera);
        }

        private int _targetIdx = 0;

        [Event(TTTEvent.Player.Died)]
        private static void OnPlayerDied(TTTPlayer deadPlayer)
        {
            if (!Host.IsClient || Local.Pawn is not TTTPlayer player)
            {
                return;
            }

            if (player.IsSpectatingPlayer && player.CurrentPlayer == deadPlayer)
            {
                player.UpdateObservatedPlayer();
            }
        }

        public void UpdateObservatedPlayer()
        {
            TTTPlayer oldObservatedPlayer = CurrentPlayer;

            CurrentPlayer = null;

            List<TTTPlayer> players = Utils.GetAlivePlayers();

            if (players.Count > 0)
            {
                if (++_targetIdx >= players.Count)
                {
                    _targetIdx = 0;
                }

                CurrentPlayer = players[_targetIdx];
            }

            if (Camera is IObservationCamera camera)
            {
                camera.OnUpdateObservatedPlayer(oldObservatedPlayer, CurrentPlayer);
            }
        }

        public void MakeSpectator(bool useRagdollCamera = true)
        {
            EnableAllCollisions = false;
            EnableDrawing = false;
            Controller = null;
            Camera = useRagdollCamera ? new RagdollSpectateCamera() : new FreeSpectateCamera();
            LifeState = LifeState.Dead;
            Health = 0f;
            ShowFlashlight(false, false);
        }

        public void ToggleForcedSpectator()
        {
            IsForcedSpectator = !IsForcedSpectator;

            if (IsForcedSpectator && LifeState == LifeState.Alive)
            {
                MakeSpectator(false);
                OnKilled();

                if (!Client.GetValue<bool>("forcedspectator", false))
                {
                    Client.SetValue("forcedspectator", true);
                }
            }
        }
    }
}
