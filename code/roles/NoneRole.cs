using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("none")]
    public class NoneRole : Role
    {
        public override Color Color => Color.Transparent;

        public override Team DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(NoneTeam));

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
