using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [Role("innocent"), Hammer.Skip]
    public class InnocentRole : Role
    {
        public override Color Color => Color.FromBytes(27, 197, 78);

        public override Team DefaultTeam { get; } = TeamFunctions.GetTeam(typeof(InnocentTeam));

        // serverside function
        public override void CreateDefaultShop()
        {
            Shop.Enabled = false;

            base.CreateDefaultShop();
        }
    }
}
