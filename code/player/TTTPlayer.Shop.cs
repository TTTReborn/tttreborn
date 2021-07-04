using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public enum BuyError
    {
        None,
        InventoryBlocked,
        NotEnoughCredits,
        RoleRestriction
    }

    public partial class TTTPlayer
    {
        public BuyError CanBuy(IBuyableItem item)
        {
            if (!item.IsBuyable(this))
            {
                return BuyError.InventoryBlocked;
            }

            if (Credits < item.Price)
            {
                return BuyError.NotEnoughCredits;
            }

            if (!Role.CanBuy())
            {
                return BuyError.RoleRestriction;
            }

            return BuyError.None;
        }

        public void RequestPurchase(IBuyableItem item)
        {
            if (CanBuy(item) == BuyError.None)
            {
                Credits -= item.Price;

                item.OnPurchase(this);

                return;
            }

            Log.Warning($"{GetClientOwner().Name} tried to buy '{item.Name}'.");
        }
    }
}
