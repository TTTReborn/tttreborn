using System;
using System.Collections.Generic;
using System.Text.Json;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Player;
using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private Panel _shopEditorWrapper;
        private List<QuickShopItem> _shopItems = new();
        private TTTRole _selectedRole;

        private void OpenShopEditor(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("ShopEditor:");

                Dropdown dropdown = panelContent.Add.Dropdown();
                dropdown.TextLabel.Text = "Choose role...";

                _shopEditorWrapper = new(panelContent);
                _shopEditorWrapper.AddClass("wrapper");

                foreach (Type roleType in Utils.GetTypes<TTTRole>())
                {
                    dropdown.AddOption(Utils.GetTypeName(roleType), roleType, (panel) =>
                    {
                        CreateShopContent(roleType);
                    });
                }

                // on select, populate
            }, "ShopEditor", "shopeditor");
        }

        private void CreateShopContent(Type roleType)
        {
            _shopEditorWrapper.DeleteChildren(true);
            _shopItems.Clear();

            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null || !role.IsSelectable)
            {
                return;
            }

            _selectedRole = role;

            foreach (Type itemType in Utils.GetTypesWithAttribute<IItem, BuyableAttribute>())
            {
                ShopItemData shopItemData = ShopItemData.CreateItemData(itemType);

                if (shopItemData == null)
                {
                    continue;
                }

                QuickShopItem item = new(_shopEditorWrapper);
                item.SetItem(shopItemData);

                item.AddEventListener("onclick", (e) =>
                {
                    ToggleItem(item, role);
                });

                item.AddEventListener("onrightclick", (e) =>
                {
                    // EditItem(item, role);
                });

                foreach (ShopItemData loopItemData in role.Shop.Items)
                {
                    if (loopItemData.Name.Equals(shopItemData.Name))
                    {
                        item.SetClass("selected", true);
                    }
                }

                _shopItems.Add(item);
            }

            // add a toggle to activate shop
            // link shops together
            // edit items
            // live update QuickShop
        }

        private static void ToggleItem(QuickShopItem item, TTTRole role)
        {
            bool toggle = !item.HasClass("selected");

            ServerUpdateItem(item.ItemData.Name, toggle, toggle ? JsonSerializer.Serialize(item.ItemData) : "", role.Name);
        }

        private static void EditItem(QuickShopItem item, TTTRole role)
        {
            if (!item.HasClass("selected"))
            {
                return;
            }

            ServerUpdateItem(item.ItemData.Name, true, JsonSerializer.Serialize(item.ItemData), role.Name);
        }

        private static bool ProcessItemUpdate(string itemName, bool toggle, string shopItemDataJson, string roleName)
        {
            Type roleType = Utils.GetTypeByName<TTTRole>(roleName);

            if (roleType == null)
            {
                return false;
            }

            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null)
            {
                return false;
            }

            ShopItemData storedItem = null;

            foreach (ShopItemData loopItem in role.Shop.Items)
            {
                if (loopItem.Name.Equals(itemName))
                {
                    storedItem = loopItem;

                    break;
                }
            }

            ShopItemData shopItemData = null;

            if (toggle)
            {
                shopItemData = JsonSerializer.Deserialize<ShopItemData>(shopItemDataJson);

                if (shopItemDataJson == null)
                {
                    return false;
                }
            }

            UpdateShop(role.Shop, toggle, storedItem, shopItemData);

            if (Host.IsServer)
            {
                foreach (Client client in Client.All)
                {
                    if (client.Pawn is TTTPlayer player && player.Role == role)
                    {
                        UpdateShop(player.Shop, toggle, storedItem, shopItemData);
                    }
                }
            }
            else if (Local.Client?.Pawn is TTTPlayer player && player.Role == role)
            {
                UpdateShop(player.Shop, toggle, storedItem, shopItemData);
            }

            return true;
        }

        private static void UpdateShop(Shop shop, bool toggle, ShopItemData storedItem, ShopItemData shopItemData)
        {
            if (toggle)
            {
                if (storedItem == null)
                {
                    shop.Items.Add(shopItemData);
                }
                else
                {
                    shop.Items.Remove(storedItem);
                    shop.Items.Add(shopItemData);
                }
            }
            else if (storedItem != null)
            {
                shop.Items.Remove(storedItem);
            }
        }

        [ServerCmd]
        public static void ServerUpdateItem(string itemName, bool toggle, string shopItemDataJson, string roleName)
        {
            if (!(ConsoleSystem.Caller?.HasPermission("shopeditor") ?? false))
            {
                return;
            }

            if (ProcessItemUpdate(itemName, toggle, shopItemDataJson, roleName))
            {
                Shop.Save(Utils.GetObjectByType<TTTRole>(Utils.GetTypeByName<TTTRole>(roleName)));

                ClientUpdateItem(itemName, toggle, shopItemDataJson, roleName);
            }
        }

        [ClientRpc]
        public static void ClientUpdateItem(string itemName, bool toggle, string shopItemDataJson, string roleName)
        {
            if (ProcessItemUpdate(itemName, toggle, shopItemDataJson, roleName))
            {
                Menu menu = Menu.Instance;

                if (menu == null || !menu.Enabled)
                {
                    return;
                }

                PanelContent menuContent = menu.MenuContent;

                if (menuContent == null || !menuContent.Title.Equals("ShopEditor") || !roleName.Equals(menu._selectedRole?.Name) || menu._shopItems.Count < 1)
                {
                    return;
                }

                foreach (QuickShopItem shopItem in menu._shopItems)
                {
                    if (shopItem.ItemData.Name.Equals(itemName))
                    {
                        shopItem.SetClass("selected", toggle);

                        break;
                    }
                }
            }
        }
    }
}
