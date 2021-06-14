namespace TTTReborn.Roles
{
    [RoleAttribute("Traitor")]
    public class TraitorRole : BaseRole
    {
        public override Color Color => Color.FromBytes(223, 41, 53);

        public TraitorRole() : base()
        {

        }
    }
}
