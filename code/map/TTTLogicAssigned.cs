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

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Rounds;
using TTTReborn.Teams;

namespace TTTReborn.Map
{
    [Library("ttt_logic_assigned", Description = "Used to test the assigned team or role of the activator.")]
    public partial class TTTLogicAssigned : Entity
    {
        [Property("Check Value", "Note that teams are often plural. For example, check the `Role` for `role_traitor`, but check the `Team` for `team_traitors`.")]
        public string CheckValue
        {
            get => _checkValue;
            set
            {
                _checkValue = value?.ToLower();
            }
        }
        private string _checkValue = Utils.GetLibraryName(typeof(TraitorTeam));

        /// <summary>
        /// Fires if activator's check type matches the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
        /// </summary>
        protected Output OnPass { get; set; }

        /// <summary>
        /// Fires if activator's check type does not match the check value. Remember that outputs are reversed. If a player's role/team is equal to the check value, the entity will trigger OnPass().
        /// </summary>
        protected Output OnFail { get; set; }

        [Input]
        public void Activate(Entity activator)
        {
            if (activator is TTTPlayer player && Gamemode.Game.Instance.Round is InProgressRound)
            {
                if (player.Role.Name.Equals(CheckValue))
                {
                    OnPass.Fire(this);

                    return;
                }
                else if (player.Team.Name.Equals(CheckValue))
                {
                    OnPass.Fire(this);

                    return;
                }

                OnFail.Fire(this);
            }
            else
            {
                Log.Warning("ttt_logic_assigned: Activator is not player.");
                OnFail.Fire(this);
            }
        }
    }
}
