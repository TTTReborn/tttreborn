using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_radar1")]
    public class Radar1 : TTTPerk, IBuyableItem
    {
        public Radar1() : base()
        {

        }

        public int Price => 100;
    }
}
