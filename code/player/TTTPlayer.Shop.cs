using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public enum BuyError
    {
        None,
        InventoryBlocked,
        NotEnoughCredits,
        NoAccess
    }

    public class Shop
    {
        public List<ShopItemData> Items { set; get; } = new();

        public Shop()
        {

        }
    }

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

                Event.Run("tttreborn.shop.change");
            }
        }
        private Shop _shop;

        public BuyError CanBuy(ShopItemData? itemData)
        {
            if (Shop == null)
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
                Log.Warning($"{GetClientOwner().Name} tried to buy '{itemData.Name}'. (Error: {buyError})");

                return;
            }

            Credits -= itemData.Price;

            buyableItem.OnPurchase(this);

            ClientSendQuickshopUpdate(To.Single(this));
        }

        [ClientRpc]
        public static void ClientSendQuickshopUpdate()
        {
            if (QuickShop.Instance?.IsShowing ?? false)
            {
                QuickShop.Instance.Update();
            }
        }

        public void ServerUpdateShop()
        {
            ClientUpdateShop(To.Single(this), JsonSerializer.Serialize<Shop>(Shop));
        }

        [ClientRpc]
        public static void ClientUpdateShop(string shopJson)
        {
            Shop shop = JsonSerializer.Deserialize<Shop>(shopJson);

            if (shop != null)
            {
                List<ShopItemData> items = new();

                foreach (ShopItemData shopItemData in shop.Items)
                {
                    IBuyableItem item = Utils.GetObjectByType<IBuyableItem>(Utils.GetTypeByName<IBuyableItem>(shopItemData.Name));
                    ShopItemData itemData = item.CreateItemData();

                    item.Delete();

                    itemData.Price = shopItemData.Price;

                    items.Add(itemData);
                }

                shop.Items = items;
            }

            (Local.Pawn as TTTPlayer).Shop = shop;
        }
    }
}
