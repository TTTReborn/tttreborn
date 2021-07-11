using System;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public struct ShopItemData
    {
        public string Name;
        public string Description;
        public int Price;
        public Type Type;

        public bool IsBuyable(TTTPlayer player)
        {
            if (Type.IsSubclassOf(typeof(TTTEquipment)))
            {
                return !(player.Inventory as Inventory).IsCarryingType(Type);
            }
            else if (Type.IsSubclassOf(typeof(TTTPerk)))
            {
                return !(player.Inventory as Inventory).Perks.Has(Name);
            }
            else if (Type.IsSubclassOf(typeof(TTTWeapon)))
            {
                return !(player.Inventory as Inventory).IsCarryingType(Type);
            }

            return false;
        }
    }

    public interface IBuyableItem : IItem
    {
        int Price { get; }

        ShopItemData CreateItemData()
        {
            ShopItemData itemData = new ShopItemData
            {
                Name = Name,
                Price = Price,
                Type = GetType()
            };

            if (this is ICarriableItem carriableItem)
            {
                itemData.Description = $"Slot: {(int) carriableItem.HoldType}";
            }

            return itemData;
        }

        void OnPurchase(TTTPlayer player)
        {
            (player.Inventory as Inventory).TryAdd(this);
        }
    }
}
