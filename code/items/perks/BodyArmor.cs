using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_perk_bodyarmor")]
    [Buyable(Price = 100)]
    [Perk]
    [Hammer.Skip]
    public partial class BodyArmor : Perk
    {
        public BodyArmor() : base()
        {

        }
    }
}
