namespace TTTReborn.Roles
{
    [RoleAttribute("None")]
    public class NoneRole : BaseRole
    {
        public override Color Color => Color.Transparent;

        public NoneRole() : base()
        {

        }
    }
}
