using Sandbox;

using TTTReborn.Player;
using TTTReborn.Gamemode;

namespace TTTReborn.Roles
{
    [RoleAttribute("Traitor")]
    public class TraitorRole : BaseRole
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public TraitorRole() : base()
        {

        }

        public override void OnSelect(TTTPlayer player)
        {
            if (Host.IsServer)
            {
                foreach (TTTPlayer otherPlayer in Gamemode.Game.GetPlayers())
                {
                    if (otherPlayer.Role is TraitorRole)
                    {
                        player.ClientSetRole(To.Single(otherPlayer), player.Role.Name);
                        otherPlayer.ClientSetRole(To.Single(player), otherPlayer.Role.Name);
                    }
                }
            }
        }
    }
}
