using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IBuyableItem : IItem
    {
        int Price { get; }

        bool IsBuyable(TTTPlayer player);

        void OnPurchase(TTTPlayer player)
        {
            if ((player.Inventory as Inventory).TryAdd(this))
            {
                Equip(player);
            }
        }
    }
}
