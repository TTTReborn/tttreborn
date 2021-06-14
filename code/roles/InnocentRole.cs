namespace TTTReborn.Roles
{
    [RoleAttribute("Innocent")]
    public class InnocentRole : BaseRole
    {
        public override Color Color => Color.FromBytes(27, 197, 78);

        public InnocentRole() : base()
        {

        }
    }
}
