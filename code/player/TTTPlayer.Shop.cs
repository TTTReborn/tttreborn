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
        public BuyError CanBuy(ShopItemData? itemData)
        {
            if (!itemData?.IsBuyable(this) ?? false)
            {
                return BuyError.InventoryBlocked;
            }

            if (Credits < itemData?.Price)
            {
                return BuyError.NotEnoughCredits;
            }

            if (!Role.CanBuy())
            {
                return BuyError.RoleRestriction;
            }

            return BuyError.None;
        }

        private void RequestPurchase(IBuyableItem buyableItem)
        {
            ShopItemData itemData = buyableItem.CreateItemData();

            BuyError buyError = CanBuy(itemData);

            if (buyError != BuyError.None)
            {
                Log.Warning($"{GetClientOwner().Name} tried to buy '{itemData.Name}'. (Error: {buyError})");

                return;
            }

            Credits -= itemData.Price;

            buyableItem.OnPurchase(this);
        }
    }
}
