using System;

using Sandbox;

using TTTReborn.Player;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BuyableAttribute : Attribute
    {
        public int Price = 100;

        public BuyableAttribute() : base()
        {

        }
    }

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

        public void CopyFrom(ShopItemData shopItemData)
        {
            Price = shopItemData.Price;
            Description = shopItemData.Description ?? Description;
            SlotType = shopItemData.SlotType ?? SlotType;
            Type = shopItemData.Type ?? Type;
        }

        public ShopItemData Clone()
        {
            ShopItemData shopItemData = new ShopItemData(Name);
            shopItemData.CopyFrom(this);

            return shopItemData;
        }

        public static ShopItemData CreateItemData(Type type)
        {
            LibraryAttribute attribute = Library.GetAttribute(type);
            bool buyable = false;

            ShopItemData shopItemData = new ShopItemData(attribute.Name)
            {
                Type = type
            };

            foreach (object obj in type.GetCustomAttributes(false))
            {
                if (obj is BuyableAttribute buyableAttribute)
                {
                    shopItemData.Price = buyableAttribute.Price;
                    buyable = true;
                }
                else if (obj is CarriableAttribute carriableAttribute)
                {
                    shopItemData.SlotType = carriableAttribute.SlotType;
                }
            }

            if (!buyable)
            {
                Log.Warning($"'{type}' is missing the 'BuyableAttribute'");

                return null;
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
            else if (Type.IsSubclassOf(typeof(TTTWeapon)))
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
}
