using System;

using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private Panel _shopEditorWrapper;

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

            TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

            if (role == null)
            {
                return;
            }

            foreach (ShopItemData shopItemData in role.Shop.Items)
            {
                QuickShopItem item = new(_shopEditorWrapper);
                item.SetItem(shopItemData);

                item.AddEventListener("onclick", () =>
                {
                    if (item.IsDisabled)
                    {
                        return;
                    }

                    item.SetClass("selected", !item.HasClass("selected"));
                });
            }
        }
    }
}
