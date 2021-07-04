using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_test2")]
    public class Test2 : TTTPerk, IBuyableItem
    {
        public Test2() : base()
        {

        }

        public int Price => 0;
    }
}
