namespace TTTReborn.Roles
{
    [RoleAttribute("Traitor")]
    public class TraitorRole : BaseRole
    {
        public override Color Color => Color.Red;

        public TraitorRole() : base()
        {

        }
    }
}
