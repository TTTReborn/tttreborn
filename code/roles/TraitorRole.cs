using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;

namespace TTTReborn.Roles
{
    [RoleAttribute("Traitor")]
    public class TraitorRole : TTTRole
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public override int DefaultCredits => 100;

        public override string DefaultTeamName => "Traitors";

        public TraitorRole() : base()
        {

        }

        public override void OnSelect(TTTPlayer player)
        {
            if (Host.IsServer && player.Team.Name == DefaultTeam.Name)
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

        public override bool CanBuy() => true;
    }
}
