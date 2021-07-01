using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IBuyableItem : IItem
    {
        int Price { get; }

        bool IsBuyable(TTTPlayer player);
    }
}
