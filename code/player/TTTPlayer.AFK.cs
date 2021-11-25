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
using System.Collections.Generic;
using System.Linq;

using Sandbox;

using TTTReborn.Settings;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public readonly static List<InputButton> Buttons = Enum.GetValues(typeof(InputButton)).Cast<InputButton>().ToList();

        private TimeSince _timeSinceLastAction = 0f;

        private void TickAFKSystem()
        {
            if (Client == null || Client.IsBot)
            {
                return;
            }

            if (IsForcedSpectator || IsSpectator)
            {
                _timeSinceLastAction = 0;

                return;
            }

            bool isAnyKeyPressed = Buttons.Any(button => Input.Down(button));
            bool isMouseMoving = Input.MouseDelta != Vector3.Zero;

            if (isAnyKeyPressed || isMouseMoving)
            {
                _timeSinceLastAction = 0f;

                return;
            }

            if (_timeSinceLastAction > ServerSettings.Instance.AFK.SecondsTillKick)
            {
                bool shouldKick = ServerSettings.Instance.AFK.ShouldKickPlayers;

                if (shouldKick)
                {
                    Log.Warning($"Player ID: {Client.PlayerId}, Name: {Client.Name} was kicked from the server for being AFK.");

                    Client.Kick();
                }
                else
                {
                    Log.Warning($"Player ID: {Client.PlayerId}, Name: {Client.Name} was moved to spectating for being AFK.");

                    ToggleForcedSpectator();
                }
            }
        }
    }
}
