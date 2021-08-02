using System;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Player;
using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [RoleAttribute("Detective")]
    public class DetectiveRole : TTTRole
    {
        public override Color Color => Color.FromBytes(28, 93, 240);

        public override int DefaultCredits => 100;

        public override Type DefaultTeamType => typeof(InnocentTeam);

        public DetectiveRole() : base()
        {

        }


        public override int NumberOfPlayersWithRole(int playerCount)
        {
            return (int) Math.Max(playerCount * 0.125f, 1f);
        }

        public override bool CanBuy() => true;
    }
}
