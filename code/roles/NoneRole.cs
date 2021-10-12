using System;

using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("none")]
    public class NoneRole : TTTRole
    {
        public override Color Color => Color.Transparent;

        public override Type DefaultTeamType => typeof(NoneTeam);

        public override bool IsSelectable => false;

        public NoneRole() : base()
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
