using Sandbox;

namespace TTTReborn.Items
{
    [Library("perk_test1")]
    [Buyable(Price = 0)]
    [Hammer.Skip]
    public class Test1 : TTTPerk
    {
        public Test1() : base()
        {

        }
    }
}
