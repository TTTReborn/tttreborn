using System;
using System.Collections.Generic;

using Sandbox;

using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.Items
{
    public struct ShopItemData
    {
        public string Name;
        public string Description;
        public int Price;
        public SlotType SlotType;
        public Type Type;
        public List<TTTRole> AvailableForRoles;

        public bool IsBuyable(TTTPlayer player)
        {
            Inventory inventory = player.Inventory as Inventory;

            if (Type.IsSubclassOf(typeof(TTTPerk)))
            {
                return !inventory.Perks.Has(Name);
            }
            else if (Type.IsSubclassOf(typeof(TTTEquipment)) || Type.IsSubclassOf(typeof(TTTWeapon)))
            {
                return !inventory.IsCarryingType(Type) && inventory.HasEmptySlot(SlotType);
            }

            return false;
        }
    }

    public interface IBuyableItem : IItem
    {
        int Price { get; }
        List<TTTRole> AvailableForRoles => new ();

        ShopItemData CreateItemData()
        {
            ShopItemData itemData = new ShopItemData
            {
                Name = Name,
                Price = Price,
                AvailableForRoles = AvailableForRoles,
                Type = GetType()
            };

            if (this is ICarriableItem carriableItem)
            {
                itemData.Description = $"Slot: { carriableItem.SlotType }";
                itemData.SlotType = carriableItem.SlotType;
            }

            return itemData;
        }

        void OnPurchase(TTTPlayer player)
        {
            (player.Inventory as Inventory).TryAdd(this);
        }
    }
}
