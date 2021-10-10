using System;
using System.Collections.Generic;

using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private Panel _shopEditorWrapper;
        private Switch _shopToggle;
        private List<QuickShopItem> _shopItems = new();
        private TTTRole _selectedRole;

        private void OpenShopEditor(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                _shopToggle = panelContent.Add.Switch("shoptoggle", false);
                _shopToggle.Disabled = true;

                _shopToggle.AddTooltip("Toggle to de-/activate the shop for the currently selected role.", "togglehint");

                Dropdown dropdown = panelContent.Add.Dropdown();
                dropdown.TextLabel.Text = "Choose role...";
                dropdown.AddTooltip("Select a role to modify the connected shop.", "roleselection");

                _shopEditorWrapper = new(panelContent);
                _shopEditorWrapper.AddClass("wrapper");

                _shopEditorWrapper.Add.Label("Please select a role to modify the connected shop.");

                foreach (Type roleType in Utils.GetTypes<TTTRole>())
                {
                    TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

                    if (role == null)
                    {
                        continue;
                    }

                    dropdown.AddOption(role.Name, role, (panel) =>
                    {
                        CreateShopContent(role);
                    });
                }
            }, "ShopEditor", "shopeditor");
        }

        private void CreateShopContent(TTTRole role)
        {
            _shopEditorWrapper.DeleteChildren(true);
            _shopItems.Clear();

            _selectedRole = role;

            _shopToggle.Disabled = false;
            _shopToggle.Checked = role.Shop.Enabled;
            _shopToggle.OnCheck = (e) =>
            {
                ServerToggleShop(role.Name, !_shopToggle.Checked);

                return false;
            };

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
                    EditItem(item, role);
                });

                foreach (ShopItemData loopItemData in role.Shop.Items)
                {
                    if (loopItemData.Name.Equals(shopItemData.Name))
                    {
                        item.SetClass("selected", true);
                    }
                }

                item.AddTooltip("", "buttons", null, (tooltip) =>
                {
                    item.SetClass("tooltip-right", false);
                    item.SetClass("tooltip-left", false);
                }, (tooltip) =>
                {
                    if (item.HasClass("selected"))
                    {
                        if (item.HasClass("tooltip-left"))
                        {
                            return;
                        }

                        item.SetClass("tooltip-right", false);
                        item.SetClass("tooltip-left", true);

                        tooltip.DeleteChildren(true);

                        Sandbox.UI.Panel panel = tooltip.Add.Panel("span");
                        panel.Add.Label("keyboard_arrow_left", "icon");
                        panel.Add.Label($"Deactivate this item in the {role.Name} shop.");

                        panel = tooltip.Add.Panel("span");
                        panel.Add.Icon("keyboard_arrow_right", "icon");
                        panel.Add.Label("Edit this item.");
                    }
                    else if (!item.HasClass("tooltip-right"))
                    {
                        item.SetClass("tooltip-right", true);
                        item.SetClass("tooltip-left", false);

                        tooltip.DeleteChildren(true);

                        Sandbox.UI.Panel panel = tooltip.Add.Panel("span");
                        panel.Add.Label("keyboard_arrow_left", "icon");
                        panel.Add.Label($"Activate this item in the {role.Name} shop.");
                    }
                });

                _shopItems.Add(item);
            }

            // link shops together
            // edit items
        }
    }
}
