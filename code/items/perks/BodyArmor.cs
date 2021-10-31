using Sandbox;

namespace TTTReborn.Items
{
    [Library("perk_bodyarmor")]
    [Buyable(Price = 100)]
    [Hammer.Skip]
    public partial class BodyArmor : TTTPerk
    {
        public BodyArmor() : base()
        {

        }
    }
}
