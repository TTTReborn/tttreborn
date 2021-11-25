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

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("traitor")]
    public class TraitorRole : TTTRole
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public override int DefaultCredits => 100;

        public override TTTTeam DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(TraitorTeam));

        public TraitorRole() : base()
        {

        }

        public override void OnSelect(TTTPlayer player)
        {
            if (Host.IsServer && player.Team == DefaultTeam)
            {
                foreach (TTTPlayer otherPlayer in player.Team.Members)
                {
                    if (otherPlayer == player)
                    {
                        continue;
                    }

                    player.SendClientRole(To.Single(otherPlayer));
                    otherPlayer.SendClientRole(To.Single(player));
                }

                foreach (TTTPlayer otherPlayer in Utils.GetPlayers())
                {
                    if (otherPlayer.IsMissingInAction)
                    {
                        otherPlayer.SyncMIA(player);
                    }
                }
            }

            base.OnSelect(player);
        }

        // serverside function
        public override void CreateDefaultShop()
        {
            Shop.AddAllItems();

            base.CreateDefaultShop();
        }

        // serverside function
        public override void UpdateDefaultShop(List<Type> newItemsList)
        {
            Shop.AddNewItems(newItemsList);

            base.UpdateDefaultShop(newItemsList);
        }
    }
}
