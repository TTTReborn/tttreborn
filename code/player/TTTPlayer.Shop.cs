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
            BuyError buyError = CanBuy(item);

            if (buyError != BuyError.None)
            {
                Log.Warning($"{GetClientOwner().Name} tried to buy '{item.Name}'. (Error: {buyError})");

                item.Delete();

                return;
            }

            Credits -= item.Price;

            item.OnPurchase(this);
        }
    }
}
