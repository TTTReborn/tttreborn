namespace TTTReborn.Roles
{
    [RoleAttribute(Name = "No Role")]
    public class NoneRole : BaseRole
    {
        public override string Name => "No Role";
    }
}
