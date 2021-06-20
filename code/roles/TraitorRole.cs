using Sandbox;

using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [RoleAttribute("Traitor")]
    public class TraitorRole : TTTRole
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public override int DefaultCredits => 100;

        public TraitorRole() : base()
        {
            DefaultTeam = TTTTeam.GetTeam("Traitors");
            DefaultTeam.Color = Color;
        }

        public override void OnSelect(TTTPlayer player)
        {
            if (Host.IsServer && player.Team != null)
            {
                foreach (TTTPlayer otherPlayer in player.Team.Members)
                {
                    player.ClientSetRole(To.Single(otherPlayer), player.Role.Name);
                    otherPlayer.ClientSetRole(To.Single(player), otherPlayer.Role.Name);
                }
            }

            base.OnSelect(player);
        }

        public override bool CanBuy() => true;
    }
}
