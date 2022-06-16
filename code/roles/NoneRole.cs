using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("none"), HideInEditor]
    public class NoneRole : Role
    {
        public override Color Color => Color.Transparent;

        public override Team DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(NoneTeam));

        public override bool IsSelectable => false;

        // serverside function
        public override void CreateDefaultShop()
        {
            Shop.Enabled = false;

            base.CreateDefaultShop();
        }
    }
}
