namespace TTTReborn.Roles
{
    [RoleAttribute("Innocent")]
    public class InnocentRole : BaseRole
    {
        public override Color Color => Color.Green;

        public InnocentRole() : base()
        {

        }
    }
}
