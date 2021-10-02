using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.Player
{
    public partial class ShopManager
    {
        public static readonly List<Type> NewItemsList = new();

        // serverside-only
        internal static void Load()
        {
            List<Type> itemList = Utils.GetTypes<IBuyableItem>();
            string fileName = $"settings/{Utils.GetTypeNameByType(typeof(Settings.ServerSettings)).ToLower()}/internal/shopitems.json";

            if (FileSystem.Data.FileExists(fileName))
            {
                List<string> loadedItems = null;

                try
                {
                    loadedItems = JsonSerializer.Deserialize<List<string>>(FileSystem.Data.ReadAllText(fileName));
                }
                catch (Exception) { }

                bool loaded = loadedItems != null && loadedItems.Count > 0;

                foreach (Type type in itemList)
                {
                    if (loaded)
                    {
                        bool found = false;
                        string availableItemName = Utils.GetTypeNameByType(type).ToLower();

                        foreach (string loadedItemName in loadedItems)
                        {
                            if (loadedItemName.Equals(availableItemName))
                            {
                                found = true;

                                break;
                            }
                        }

                        if (found)
                        {
                            continue;
                        }
                    }

                    NewItemsList.Add(type);
                }
            }

            if (NewItemsList.Count > 0)
            {
                CreateItemsFile(fileName, itemList);
            }
        }

        private static void CreateItemsFile(string fileName, List<Type> itemList)
        {
            List<string> availableItems = new();

            foreach (Type type in itemList)
            {
                availableItems.Add(Utils.GetTypeNameByType(type).ToLower());
            }

            Utils.CreateRecursiveDirectories(fileName);

            try
            {
                FileSystem.Data.DeleteFile(fileName);
            }
            catch (Exception) { }

            FileSystem.Data.WriteAllText(fileName, JsonSerializer.Serialize(availableItems));
        }
    }
}
