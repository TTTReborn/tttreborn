using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;

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

        public bool Accessable()
        {
            return Items.Count > 0;
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

                    // create clean instance
                    ShopItemData itemData = ShopItemData.CreateItemData(itemType);

                    if (itemData == null)
                    {
                        continue;
                    }

                    // override with settings data
                    itemData.Price = shopItemData.Price;

                    items.Add(itemData);
                }

                shop.Items = items;
            }

            return shop;
        }

        public static void Load(TTTRole role)
        {
            string fileName = GetSettingsFile(role);

            if (!FileSystem.Data.FileExists(fileName))
            {
                role.Shop = new();

                role.CreateDefaultShop();
                Utils.CreateRecursiveDirectories(fileName);

                Save(fileName, role);

                return;
            }

            role.Shop = Shop.InitializeFromJSON(FileSystem.Data.ReadAllText(fileName));

            if (ShopManager.NewItemsList.Count > 0)
            {
                role.UpdateDefaultShop(ShopManager.NewItemsList);

                Save(fileName, role);
            }
        }

        public static void Save(string fileName, TTTRole role)
        {
            FileSystem.Data.WriteAllText(fileName, JsonSerializer.Serialize(role.Shop, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        public static string GetSettingsFile(TTTRole role)
        {
            return $"settings/{Utils.GetTypeNameByType(typeof(Settings.ServerSettings)).ToLower()}/shop/{role.Name.ToLower()}.json";
        }

        internal void AddAllItems()
        {
            Items.Clear();

            foreach (Type itemType in Utils.GetTypes<IBuyableItem>())
            {
                Items.Add(ShopItemData.CreateItemData(itemType));
            }
        }

        internal void AddNewItems(List<Type> newItemsList)
        {
            List<string> storedItemList = new();

            foreach (ShopItemData shopItemData in Items)
            {
                storedItemList.Add(Utils.GetTypeNameByType(shopItemData.Type).ToLower());
            }

            foreach (Type type in newItemsList)
            {
                bool found = false;
                string newItemName = Utils.GetTypeNameByType(type).ToLower();

                foreach (string storedItemName in storedItemList)
                {
                    if (newItemName.Equals(storedItemName))
                    {
                        found = true;

                        break;
                    }
                }

                if (found)
                {
                    continue;
                }

                Items.Add(ShopItemData.CreateItemData(type));
            }
        }
    }
}
