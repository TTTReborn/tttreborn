using System;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [RoleAttribute("Traitor")]
    public class TraitorRole : TTTRole
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public override int DefaultCredits => 100;

        public override Type DefaultTeamType => typeof(TraitorTeam);

        public TraitorRole() : base()
        {

        }

        public override void OnSelect(TTTPlayer player)
        {
            if (Host.IsServer && player.Team.GetType() == DefaultTeamType)
            {
                foreach (TTTPlayer otherPlayer in player.Team.Members)
                {
                    RPCs.ClientSetRole(To.Single(otherPlayer), player, player.Role.Name);
                    RPCs.ClientSetRole(To.Single(player), otherPlayer, otherPlayer.Role.Name);
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
        public override void InitShop()
        {
            base.InitShop();

            if (Shop != null)
            {
                return;
            }

            // init default shop
            Shop = new Shop();

            foreach (Type itemType in Utils.GetTypes<IBuyableItem>())
            {
                IBuyableItem item = Utils.GetObjectByType<IBuyableItem>(itemType);
                Shop.Items.Add(item.CreateItemData());

                item.Delete();
            }
        }
    }
}
