namespace TTTReborn.Roles
{
    [RoleAttribute("Innocent")]
    public class InnocentRole : TTTRole
    {
        public override Color Color => Color.FromBytes(27, 197, 78);

        public override string DefaultTeamName => "Innocents";

        public InnocentRole() : base()
        {

        }
    }
}
