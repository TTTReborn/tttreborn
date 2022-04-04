using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("traitor"), Hammer.Skip]
    public class TraitorRole : Role
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public override int DefaultCredits => 100;

        public override Team DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(TraitorTeam));

        public override void OnSelect(Player player)
        {
            if (Host.IsServer && player.Team == DefaultTeam)
            {
                foreach (Player otherPlayer in player.Team.Members)
                {
                    if (otherPlayer == player)
                    {
                        continue;
                    }

                    player.SendClientRole(To.Single(otherPlayer));
                    otherPlayer.SendClientRole(To.Single(player));
                }

                foreach (Player otherPlayer in Utils.GetPlayers())
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
