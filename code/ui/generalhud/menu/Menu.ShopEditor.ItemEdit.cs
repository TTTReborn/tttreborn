using System.Text.Json;

using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private DialogBox _currentDialogBox;
        private ShopItemData _shopItemData;

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
            if (Menu.Instance._currentDialogBox != null)
            {
                Menu.Instance._currentDialogBox.Delete(true);
            }

            DialogBox dialogBox = new DialogBox();
            dialogBox.Header.DragHeader.IsLocked = false;
            dialogBox.SetTitle($"Edit item '{item.ItemData.Name}'");
            dialogBox.AddClass("itemeditwindow");

            dialogBox.OnAgree = () =>
            {
                ShopItemData shopItemData = Menu.Instance._shopItemData;

                ServerUpdateItem(shopItemData.Name, true, JsonSerializer.Serialize(shopItemData), role.Name);

                dialogBox.Close();
            };

            dialogBox.OnDecline = () =>
            {
                Menu.Instance._shopItemData = item.ItemData;

                dialogBox.Close();
            };

            Menu.Instance._currentDialogBox = dialogBox;
            Menu.Instance._shopItemData = item.ItemData.Clone();

            PopulateEditWindowWithSettings();

            return dialogBox;
        }

        private static void PopulateEditWindowWithSettings()
        {
            Menu.Instance._currentDialogBox.Content.SetPanelContent((panelContent) =>
            {
                ShopItemData shopItemData = Menu.Instance._shopItemData;

                CreateSettingsEntry(panelContent, "Price", shopItemData.Price, $"The price of the '{shopItemData.Name}'.", null, (value) =>
                {
                    Menu.Instance._shopItemData.Price = value;
                });
            });
        }
    }
}
