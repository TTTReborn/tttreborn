using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IBuyableItem : IItem
    {
        int GetPrice();

        bool IsBuyable(TTTPlayer player);
    }
}
