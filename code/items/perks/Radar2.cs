using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_radar2")]
    public class Radar2 : TTTPerk, IBuyableItem
    {
        public Radar2() : base()
        {

        }

        public int Price => 100;
    }
}
