using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public interface IBuyableItem : IItem
    {
        int Price { get; }

        bool IsBuyable(TTTPlayer player);

        void OnPurchase(TTTPlayer player)
        {
            (player.Inventory as Inventory).Add(this);

            Equip(player);
        }
    }
}
