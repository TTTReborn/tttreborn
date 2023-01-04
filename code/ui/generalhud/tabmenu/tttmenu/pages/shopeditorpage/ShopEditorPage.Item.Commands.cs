using System;
using System.Text.Json;

using Sandbox;
using Sandbox.UI;

using TTTReborn.Globalization;
using TTTReborn.Items;
using TTTReborn.Roles;
using TTTReborn.UI.Menu;

namespace TTTReborn.UI
{
    public partial class ShopEditorPage : Panel
    {
        private static void ToggleItem(QuickShopItem item, Role role)
        {
            bool toggle = !item.HasClass("selected");

            ServerUpdateItem(item.ItemData.Name, toggle, toggle ? JsonSerializer.Serialize(item.ItemData) : "", role.Name);
        }

        [ConCmd.Server]
        public static void ServerUpdateItem(string itemName, bool toggle, string shopItemDataJson, string roleName)
        {
            //if (!(ConsoleSystem.Caller?.HasPermission("shopeditor") ?? false))
            //{
            //    return;
            //}

            if (ProcessItemUpdate(itemName, toggle, shopItemDataJson, roleName, out _))
            {
                Shop.Save(Utils.GetObjectByType<Role>(Utils.GetTypeByLibraryName<Role>(roleName)));

                ClientUpdateItem(itemName, toggle, shopItemDataJson, roleName);
            }
        }

        private static bool ProcessItemUpdate(string itemName, bool toggle, string shopItemDataJson, string roleName, out ShopItemData shopItemData)
        {
            shopItemData = null;

            Type roleType = Utils.GetTypeByLibraryName<Role>(roleName);

            if (roleType == null)
            {
                return false;
            }

            Role role = Utils.GetObjectByType<Role>(roleType);

            if (role == null)
            {
                return false;
            }

            if (toggle)
            {
                ShopItemData itemData = JsonSerializer.Deserialize<ShopItemData>(shopItemDataJson);

                if (itemData == null)
                {
                    return false;
                }

                Type itemType = Utils.GetTypeByLibraryName<IItem>(itemName);

                if (itemType == null)
                {
                    return false;
                }

                shopItemData = ShopItemData.CreateItemData(itemType);

                if (shopItemData == null)
                {
                    return false;
                }

                shopItemData.CopyFrom(itemData);
            }

            UpdateShop(role.Shop, toggle, itemName, shopItemData);

            if (Game.IsServer)
            {
                foreach (IClient client in Game.Clients)
                {
                    if (client.Pawn is Player player && player.Role.Equals(roleName))
                    {
                        UpdateShop(player.Shop, toggle, itemName, shopItemData);
                    }
                }
            }
            else if (Game.LocalClient?.Pawn is Player player && player.Role.Name.Equals(roleName))
            {
                UpdateShop(player.Shop, toggle, itemName, shopItemData);

                QuickShop.Instance?.Reload();
            }

            return true;
        }

        private static void UpdateShop(Shop shop, bool toggle, string itemName, ShopItemData shopItemData)
        {
            ShopItemData storedItem = null;

            foreach (ShopItemData loopItem in shop.Items)
            {
                if (loopItem.Name.Equals(itemName))
                {
                    storedItem = loopItem;

                    break;
                }
            }

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

        [ClientRpc]
        public static void ClientUpdateItem(string itemName, bool toggle, string shopItemDataJson, string roleName)
        {
            if (!ProcessItemUpdate(itemName, toggle, shopItemDataJson, roleName, out ShopItemData shopItemData))
            {
                return;
            }

            if (TTTMenu.Instance.ActivePage is not ShopEditorPage shopEditorPage)
            {
                return;
            }

            foreach (QuickShopItem shopItem in shopEditorPage.ShopItems)
            {
                if (shopItem.ItemData.Name.Equals(itemName))
                {
                    if (shopItemData != null)
                    {
                        shopItem.SetItem(shopItemData);
                    }

                    shopItem.SetClass("selected", toggle);

                    break;
                }
            }
        }

        private static void EditItem(QuickShopItem item, Role role)
        {
            Modal itemEditModal = CreateItemEditModal(item, role);

            Hud.Current.RootPanel.AddChild(itemEditModal);

            itemEditModal.Display();
        }

        private static Modal CreateItemEditModal(QuickShopItem item, Role role)
        {
            DialogBox dialogBox = new();
            dialogBox.Header.DragHeader.IsLocked = false;
            dialogBox.SetTranslationTitle(new TranslationData("MENU.SHOPEDITOR.EDIT.DESCRIPTION", new TranslationData(item.ItemData.GetTranslationKey("NAME"))));
            dialogBox.AddClass("itemeditwindow");

            dialogBox.OnAgree = () =>
            {
                ServerUpdateItem(item.ItemData.Name, true, JsonSerializer.Serialize(item.ItemData), role.Name);

                dialogBox.Close();
            };

            dialogBox.OnDecline = () =>
            {
                dialogBox.Close();
            };

            PopulateEditWindowWithSettings(dialogBox, item);

            return dialogBox;
        }

        private static void PopulateEditWindowWithSettings(DialogBox dialog, QuickShopItem item)
        {
            dialog.Content.SetPanelContent((panelContent) =>
            {
                SettingsPage.CreateSettingsEntry(panelContent, "MENU.SHOPEDITOR.PRICE.TITLE", item.ItemData.Price, "MENU.SHOPEDITOR.PRICE.DESCRIPTION", null, (value) =>
                {
                    item.ItemData.Price = value;
                });
            });
        }
    }
}
