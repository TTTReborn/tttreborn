using TTTReborn.Teams;

namespace TTTReborn.Roles
{
    [RoleAttribute("Innocent")]
    public class InnocentRole : TTTRole
    {
        public override Color Color => Color.FromBytes(27, 197, 78);

        public InnocentRole() : base()
        {
            DefaultTeam = InnocentTeam.Instance;
        }
    }
}
