using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Items;
using TTTReborn.Rounds;
using TTTReborn.UI;

#pragma warning disable IDE0051

namespace TTTReborn
{
    public partial class Player
    {
        public HashSet<string> BoughtItemsSet = new();

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

                GameEvent.Register(new Events.Shop.ChangeEvent());
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

            if (itemData.IsLimited && BoughtItemsSet.Contains(itemData.Name))
            {
                return BuyError.LimitReached;
            }

            return BuyError.None;
        }

        public void RequestPurchase(Type itemType)
        {
            ShopItemData itemData = ShopItemData.CreateItemData(itemType);

            foreach (ShopItemData loopItemData in Shop.Items)
            {
                if (loopItemData.Name.Equals(itemData.Name))
                {
                    itemData.CopyFrom(loopItemData);
                }
            }

            BuyError buyError = CanBuy(itemData);

            if (buyError != BuyError.None)
            {
                Log.Warning($"{Client.Name} tried to buy '{itemData.Name}'. (Error: {buyError})");

                return;
            }

            Credits -= itemData.Price;

            Utils.GetObjectByType<IItem>(itemType).OnPurchase(this);
            BoughtItemsSet.Add(itemData.Name);

            ClientBoughtItem(To.Single(this), itemData.Name);
        }

        [Event("game_roundchange")]
        private static void OnRoundChanged(BaseRound _, BaseRound newRound)
        {
            if (newRound is PreRound preRound)
            {
                foreach (Player player in Utils.GetPlayers())
                {
                    player.BoughtItemsSet.Clear();
                }
            }
        }

        [ClientRpc]
        public static void ClientBoughtItem(string itemName)
        {
            (Local.Pawn as Player).BoughtItemsSet.Add(itemName);

            UpdateQuickShop();
        }

        [ClientRpc]
        public static void ClientSendQuickShopUpdate()
        {
            UpdateQuickShop();
        }

        private static void UpdateQuickShop()
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
            (Local.Pawn as Player).Shop = Shop.InitializeFromJSON(shopJson);
        }
    }
}
