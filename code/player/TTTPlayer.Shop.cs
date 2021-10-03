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

            return BuyError.None;
        }

        public void RequestPurchase(IBuyableItem buyableItem)
        {
            ShopItemData itemData = buyableItem.CreateItemData();
            BuyError buyError = CanBuy(itemData);

            if (buyError != BuyError.None)
            {
                Log.Warning($"{Client.Name} tried to buy '{itemData.Name}'. (Error: {buyError})");

                return;
            }

            Credits -= itemData.Price;

            buyableItem.OnPurchase(this);

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
