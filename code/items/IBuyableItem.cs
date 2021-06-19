using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IBuyableItem
    {
        int GetPrice();

        bool IsBuyable(TTTPlayer player);

        string GetName();

        void Equip(TTTPlayer player);
    }
}
