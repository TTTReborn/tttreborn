using Sandbox;

namespace TTTReborn.Items
{
    [Library("ttt_test1")]
    public class Test1 : TTTPerk, IBuyableItem
    {
        public Test1() : base()
        {

        }

        public int Price => 0;
    }
}
