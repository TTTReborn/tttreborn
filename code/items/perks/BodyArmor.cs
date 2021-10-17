using Sandbox;

namespace TTTReborn.Items
{
    [Buyable(Price = 0)]
    [Library("ttt_bodyarmor")]
    public partial class BodyArmor : TTTPerk
    {
        public override string DisplayName => "Body Armor";
        public BodyArmor() : base()
        {

        }
    }
}
