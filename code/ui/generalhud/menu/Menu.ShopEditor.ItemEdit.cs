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

            Hud.Current.RootPanel.AddChild(CreateItemEditWindow(item, role));
        }

        private static Window CreateItemEditWindow(QuickShopItem item, TTTRole role)
        {
            if (Menu.Instance._currentDialogBox != null)
            {
                Menu.Instance._currentDialogBox.Delete(true);
            }

            DialogBox dialogBox = new DialogBox();
            dialogBox.WindowHeader.DragHeaderWrapper.IsLocked = false;
            dialogBox.WindowHeader.NavigationHeader.SetTitle($"Edit item '{item.ItemData.Name}'");

            dialogBox.AddClass("itemeditwindow");

            dialogBox.OnAgree = () =>
            {
                ShopItemData shopItemData = Menu.Instance._shopItemData;

                ServerUpdateItem(shopItemData.Name, true, JsonSerializer.Serialize(shopItemData), role.Name);

                dialogBox.Close(true);
            };

            dialogBox.OnDecline = () =>
            {
                Menu.Instance._shopItemData = item.ItemData;

                dialogBox.Close(true);
            };

            Menu.Instance._currentDialogBox = dialogBox;
            Menu.Instance._shopItemData = item.ItemData.Clone();

            PopulateEditWindowWithSettings();

            return dialogBox;
        }

        private static void PopulateEditWindowWithSettings()
        {
            Menu.Instance._currentDialogBox.WindowContent.SetPanelContent((panelContent) =>
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
