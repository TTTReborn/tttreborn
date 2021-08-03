using Sandbox;

using TTTReborn.Items;

namespace TTTReborn.Player
{
    public enum BuyError
    {
        None,
        InventoryBlocked,
        NotEnoughCredits,
        ShopRestriction,
        RoleRestriction,
        Error,
    }

    public partial class TTTPlayer
    {
        public BuyError CanBuy(ShopItemData? itemData)
        {
            if (itemData == null)
            {
                return BuyError.Error;
            }

            if (!itemData.Value.IsBuyable(this))
            {
                return BuyError.InventoryBlocked;
            }

            if (Credits < itemData.Value.Price)
            {
                return BuyError.NotEnoughCredits;
            }

            if (!Role.CanAccessQuickShop())
            {
                return BuyError.ShopRestriction;
            }

            if (!itemData.Value.AvailableForRoles.Exists((r) => r.Name == Role.Name))
            {
                return BuyError.RoleRestriction;
            }

            return BuyError.None;
        }

        public void RequestPurchase(IBuyableItem buyableItem)
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
