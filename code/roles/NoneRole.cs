namespace TTTReborn.Roles
{
    [RoleAttribute("None")]
    public class NoneRole : TTTRole
    {
        public override Color Color => Color.Transparent;

        public override string DefaultTeamName => "Nones";

        public NoneRole() : base()
        {

        }
    }
}
