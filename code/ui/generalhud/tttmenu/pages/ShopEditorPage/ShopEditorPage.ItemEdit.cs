using System;
using System.Text.Json;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class ShopEditorPage
    {
        public DialogBox _currentDialogBox;
        public ShopItemData _shopItemData;

        private static void EditItem(QuickShopItem item, TTTRole role)
        {
            if (!item.HasClass("selected"))
            {
                return;
            }

            Modal itemEditModal = CreateItemEditModal(item, role);

            Hud.Current.RootPanel.AddChild(itemEditModal);

            itemEditModal.Display();
        }

        private static Modal CreateItemEditModal(QuickShopItem item, TTTRole role)
        {
            if (TTTMenu.Instance.ActivePage is not ShopEditorPage shopEditorPage)
            {
                return new DialogBox();
            }

            shopEditorPage._currentDialogBox?.Delete(true);

            DialogBox dialogBox = new();
            dialogBox.Header.DragHeader.IsLocked = false;
            dialogBox.SetTranslationTitle("MENU_SHOPEDITOR_ITEM_EDIT_SPECIFIC", new TranslationData(item.ItemData.Name.ToUpper()));
            dialogBox.AddClass("itemeditwindow");

            dialogBox.OnAgree = () =>
            {
                ShopItemData shopItemData = shopEditorPage._shopItemData;

                ServerUpdateItem(shopItemData.Name, true, JsonSerializer.Serialize(shopItemData), role.Name);

                dialogBox.Close();
            };

            dialogBox.OnDecline = () =>
            {
                shopEditorPage._shopItemData = item.ItemData;

                dialogBox.Close();
            };

            shopEditorPage._currentDialogBox = dialogBox;
            shopEditorPage._shopItemData = item.ItemData.Clone();

            PopulateEditWindowWithSettings();

            return dialogBox;
        }

        private static void PopulateEditWindowWithSettings()
        {
            ShopEditorPage shopEditorPage = TTTMenu.Instance.ActivePage as ShopEditorPage;

            shopEditorPage._currentDialogBox.Content.SetPanelContent((panelContent) =>
            {
                ShopItemData shopItemData = shopEditorPage._shopItemData;

                TTTMenu.CreateSettingsEntry(panelContent, "MENU_SHOPEDITOR_ITEM_PRICE", shopItemData.Price, "MENU_SHOPEDITOR_ITEM_PRICE_SPECIFIC", null, (value) =>
                {
                    shopEditorPage._shopItemData.Price = value;
                }, new TranslationData(shopItemData.Name.ToUpper()));
            });
        }
    }
}
