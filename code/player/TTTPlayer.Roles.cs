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

using TTTReborn.Globals;
using TTTReborn.Roles;
using TTTReborn.Rounds;
using TTTReborn.Teams;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public TTTRole Role
        {
            get
            {
                if (_role == null)
                {
                    _role = new NoneRole();
                }

                return _role;
            }
            private set
            {
                _role = value;
            }
        }

        private TTTRole _role;

        public TTTTeam Team
        {
            get
            {
                if (_team == null)
                {
                    _team = TeamFunctions.GetTeam(typeof(NoneTeam));
                }

                return _team;
            }
            private set
            {
                _team = value;
            }
        }

        private TTTTeam _team;

        public void SetRole(TTTRole role, TTTTeam team = null)
        {
            TTTTeam oldTeam = Team;

            Role?.OnDeselect(this);

            Role = role;
            Team = team ?? Role.DefaultTeam;

            if (oldTeam != Team)
            {
                oldTeam?.Members.Remove(this);
                Team?.Members.Add(this);
            }

            Role.OnSelect(this);
        }

        /// <summary>
        /// Sends the role + team and all connected additional data like logic buttons of the current TTTPlayer to the given target or - if no target was provided - the player itself
        /// </summary>
        /// <param name="to">optional - The target</param>
        public void SendClientRole(To? to = null)
        {
            RPCs.ClientSetRole(to ?? To.Single(this), this, Role.Name);

            if (to == null || to.Value.ToString().Equals(Client.Name))
            {
                SendLogicButtonsToClient();
            }
        }

        public void SyncMIA(TTTPlayer player = null)
        {
            if (Gamemode.Game.Instance.Round is not InProgressRound)
            {
                return;
            }

            if (player == null)
            {
                List<Client> traitors = new();

                foreach (Client client in Client.All)
                {
                    if ((client.Pawn as TTTPlayer).Team.GetType() == typeof(TraitorTeam))
                    {
                        traitors.Add(client);
                    }
                }

                RPCs.ClientAddMissingInAction(To.Multiple(traitors), this);
            }
            else
            {
                RPCs.ClientAddMissingInAction(To.Single(player), this);
            }
        }
    }
}
