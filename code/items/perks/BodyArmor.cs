using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_bodyarmor")]
    public partial class BodyArmor : TTTPerk, IBuyableItem
    {
        public int Price => 0;

        public BodyArmor() : base()
        {

        }
    }
}
