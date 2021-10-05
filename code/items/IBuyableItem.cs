using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    public class ShopItemData
    {
        public string Name { get; set; }
        public string Description = "";
        public int Price { get; set; } = 0;
        public SlotType? SlotType = null;
        public Type Type = null;

        public ShopItemData(string name)
        {
            Name = name;
        }

        public static ShopItemData CreateItemData(Type type)
        {
            LibraryAttribute attribute = Library.GetAttribute(type);

            if (attribute is not BuyableItemAttribute buyableAttribute)
            {
                Log.Warning($"'{type}' is missing the 'BuyableAttribute'");

                return null;
            }

            ShopItemData shopItemData = new ShopItemData(attribute.Name)
            {
                Price = buyableAttribute.Price,
                Type = type
            };

            if (attribute is CarriableAttribute carriableAttribute)
            {
                shopItemData.SlotType = carriableAttribute.SlotType;
            }

            return shopItemData;
        }

        public bool IsBuyable(TTTPlayer player)
        {
            if (Type.IsSubclassOf(typeof(TTTPerk)))
            {
                return !player.Inventory.Perks.Has(Name);
            }

            if (SlotType == null)
            {
                return false;
            }

            if (Type.IsSubclassOf(typeof(TTTEquipment)) || Type.IsSubclassOf(typeof(TTTWeapon)))
            {
                return !player.Inventory.IsCarryingType(Type) && player.Inventory.HasEmptySlot(SlotType.Value);
            }
            else if (Type.IsSubclassOf(typeof(TTTWeapon)))
            {
                return !player.Inventory.IsCarryingType(Type) && player.Inventory.HasEmptySlot(SlotType.Value);
            }
            else if (Type.IsSubclassOf(typeof(TTTEquipment)))
            {
                return player.Inventory.HasEmptySlot(SlotType.Value);
            }

            return false;
        }
    }

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BuyableItemAttribute : ItemAttribute
    {
        public int Price = 100;

        public BuyableItemAttribute(string name) : base(name)
        {

        }
    }

    public interface IBuyableItem : IItem
    {
        void OnPurchase(TTTPlayer player)
        {
            player.Inventory.TryAdd(this);
        }
    }
}
