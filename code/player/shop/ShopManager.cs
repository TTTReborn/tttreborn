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

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.Player
{
    public partial class ShopManager
    {
        public static List<Type> NewItemsList { get; private set; } = new();

        // serverside-only
        internal static void Load()
        {
            CheckNewItems();

            InitializeShops();
        }

        private static void CheckNewItems()
        {
            List<Type> itemList = Utils.GetTypesWithAttribute<IItem, BuyableAttribute>();
            List<string> loadedItems = null;
            string fileName = $"settings/{Utils.GetTypeName(typeof(Settings.ServerSettings)).ToLower()}/internal/shopitems.json";

            if (FileSystem.Data.FileExists(fileName))
            {
                try
                {
                    loadedItems = JsonSerializer.Deserialize<List<string>>(FileSystem.Data.ReadAllText(fileName));
                }
                catch (Exception) { }
            }

            if (loadedItems != null && loadedItems.Count > 0)
            {
                foreach (Type type in itemList)
                {
                    bool found = false;
                    string availableItemName = Utils.GetLibraryName(type);

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

                    NewItemsList.Add(type);
                }
            }
            else
            {
                NewItemsList = itemList;
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
                availableItems.Add(Utils.GetLibraryName(type));
            }

            Utils.CreateRecursiveDirectories(fileName);

            try
            {
                FileSystem.Data.DeleteFile(fileName);
            }
            catch (Exception) { }

            FileSystem.Data.WriteAllText(fileName, JsonSerializer.Serialize(availableItems));
        }

        private static void InitializeShops()
        {
            foreach (Type roleType in Utils.GetTypes<TTTRole>())
            {
                Utils.GetObjectByType<TTTRole>(roleType).InitShop();
            }
        }
    }
}
