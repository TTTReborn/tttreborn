// TTT Reborn https://github.com/TTTReborn/tttreborn/
// Copyright (C) Neoxult

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see https://github.com/TTTReborn/tttreborn/blob/master/LICENSE.

using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Events;
using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Rounds;
using TTTReborn.UI;

namespace TTTReborn.Player
{
    public partial class TTTPlayer
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

        [Event(TTTEvent.Game.RoundChange)]
        private static void OnRoundChanged(BaseRound oldRound, BaseRound newRound)
        {
            if (newRound is PreRound preRound)
            {
                foreach (TTTPlayer player in Utils.GetPlayers())
                {
                    player.BoughtItemsSet.Clear();
                }
            }
        }

        [ClientRpc]
        public static void ClientBoughtItem(string itemName)
        {
            (Local.Pawn as TTTPlayer).BoughtItemsSet.Add(itemName);

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
            (Local.Pawn as TTTPlayer).Shop = Shop.InitializeFromJSON(shopJson);
        }
    }
}
