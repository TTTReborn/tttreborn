using System.Text.Json;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Items;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
    {
        public Shop Shop
        {
            get => _shop;
            set
            {
                if (_shop == value)
                {
                    return;
                }

                _shop = value;

                Event.Run(TTTEvent.Shop.Change);
            }
        }
        private Shop _shop;

        public BuyError CanBuy(ShopItemData itemData)
        {
            if (Shop == null || !Shop.Accessable())
            {
                return BuyError.NoAccess;
            }

            if (!itemData?.IsBuyable(this) ?? false)
            {
                return BuyError.InventoryBlocked;
            }

            if (Credits < itemData?.Price)
            {
                return BuyError.NotEnoughCredits;
            }

            bool found = false;

            foreach (ShopItemData shopItemData in Shop.Items)
            {
                if (shopItemData.Name.Equals(itemData.Name))
                {
                    found = true;

                    break;
                }
            }

            if (!found)
            {
                return BuyError.NotAvailable;
            }

            return BuyError.None;
        }

        public void RequestPurchase(IItem item)
        {
            ShopItemData itemData = ShopItemData.CreateItemData(item.GetType());
            BuyError buyError = CanBuy(itemData);

            if (buyError != BuyError.None)
            {
                Log.Warning($"{Client.Name} tried to buy '{itemData.Name}'. (Error: {buyError})");

                return;
            }

            Credits -= itemData.Price;

            item.OnPurchase(this);

            ClientSendQuickshopUpdate(To.Single(this));
        }

        [ClientRpc]
        public static void ClientSendQuickshopUpdate()
        {
            if (QuickShop.Instance?.Enabled ?? false)
            {
                QuickShop.Instance.Update();
            }
        }

        public void ServerUpdateShop()
        {
            ClientUpdateShop(To.Single(this), JsonSerializer.Serialize(Shop));
        }

        [ClientRpc]
        public static void ClientUpdateShop(string shopJson)
        {
            (Local.Pawn as TTTPlayer).Shop = Shop.InitializeFromJSON(shopJson);
        }
    }
}
