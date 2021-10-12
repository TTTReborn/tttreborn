using System;

using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("innocent")]
    public class InnocentRole : TTTRole
    {
        public override Color Color => Color.FromBytes(27, 197, 78);

        public override Type DefaultTeamType => typeof(InnocentTeam);

        public InnocentRole() : base()
        {

        }

        // serverside function
        public override void CreateDefaultShop()
        {
            Shop.Enabled = false;

            base.CreateDefaultShop();
        }
    }
}
