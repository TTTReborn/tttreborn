using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Events;
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

        public static Shop InitializeFromJSON(string json)
        {
            Shop shop = JsonSerializer.Deserialize<Shop>(json);

            if (shop != null)
            {
                List<ShopItemData> items = new();

                foreach (ShopItemData shopItemData in shop.Items)
                {
                    Type itemType = Utils.GetTypeByName<IBuyableItem>(shopItemData.Name);

                    if (itemType == null)
                    {
                        continue;
                    }

                    IBuyableItem item = Utils.GetObjectByType<IBuyableItem>(itemType);
                    ShopItemData itemData = item.CreateItemData();

                    item.Delete();

                    itemData.Price = shopItemData.Price;

                    items.Add(itemData);
                }

                shop.Items = items;
            }

            // TODO add new items by default as well

            return shop;
        }

        public bool Accessable()
        {
            return Items.Count > 0;
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
