using System;

using Sandbox;

namespace TTTReborn.Items
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class BuyableAttribute : Attribute
    {
        public int Price = 100;
    }

    public class ShopItemData
    {
        public string Name { get; set; }
        public string Description = "";
        public int Price { get; set; } = 0;
        public CarriableCategories? Category = null;
        public Type Type = null;
        public bool IsLimited { get; set; } = true;

        public ShopItemData(string name)
        {
            Name = name;
        }

        public void CopyFrom(ShopItemData shopItemData)
        {
            Price = shopItemData.Price;
            Description = shopItemData.Description ?? Description;
            Category = shopItemData.Category ?? Category;
            Type = shopItemData.Type ?? Type;
            IsLimited = shopItemData.IsLimited;
        }

        public ShopItemData Clone()
        {
            ShopItemData shopItemData = new(Name);
            shopItemData.CopyFrom(this);

            return shopItemData;
        }

        public static ShopItemData CreateItemData(Type type)
        {
            TypeDescription typeDescription = TypeLibrary.GetDescription(type);
            bool buyable = false;

            ShopItemData shopItemData = new(typeDescription.ClassName)
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
                    shopItemData.Category = carriableAttribute.Category;
                }
            }

            if (!buyable)
            {
                Log.Warning($"'{type}' is missing the 'BuyableAttribute'");

                return null;
            }

            return shopItemData;
        }

        public bool IsBuyable(Player player)
        {
            if (Type.IsSubclassOf(typeof(Perk)))
            {
                return !player.Inventory.Perks.Has(Name);
            }
            else if (Category == null)
            {
                return false;
            }
            else if (Type.IsSubclassOf(typeof(Weapon)))
            {
                return !player.Inventory.IsCarryingType(Type) && player.Inventory.HasEmptySlot(Category.Value);
            }
            else if (Type.IsSubclassOf(typeof(Equipment)))
            {
                return player.Inventory.HasEmptySlot(Category.Value);
            }

            return false;
        }

        public string GetTranslationKey(string key) => Utils.GetTranslationKey(Name, key);
    }
}
