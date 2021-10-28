using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("detective")]
    public class DetectiveRole : TTTRole
    {
        public override Color Color => Color.FromBytes(25, 102, 255);

        public override int DefaultCredits => 100;

        public override Type DefaultTeamType => typeof(InnocentTeam);

        public DetectiveRole() : base()
        {

        }

        public override void OnSelect(TTTPlayer player)
        {
            if (Host.IsServer && player.Team.GetType() == DefaultTeamType)
            {
                foreach (TTTPlayer otherPlayer in Utils.GetPlayers((pl) => pl != player))
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
