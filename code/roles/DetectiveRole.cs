using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("detective"), HideInEditor]
    public class DetectiveRole : Role
    {
        public override Color Color => Color.FromBytes(25, 102, 255);

        public override int DefaultCredits => 100;

        public override Team DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(InnocentTeam));

        public override void OnSelect(Player player)
        {
            if (Host.IsServer && player.Team == DefaultTeam)
            {
                foreach (Player otherPlayer in Utils.GetPlayers((pl) => pl != player))
                {
                    player.SendClientRole(To.Single(otherPlayer));
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
