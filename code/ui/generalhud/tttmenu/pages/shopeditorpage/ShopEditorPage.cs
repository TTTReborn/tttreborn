using System;
using System.Collections.Generic;

using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Items;
using TTTReborn.Roles;

namespace TTTReborn.UI
{
    [UseTemplate]
    public partial class ShopEditorPage : Panel
    {
        public readonly List<QuickShopItem> ShopItems = new();

        private Panel Controls { get; set; }
        private Panel RoleShopContent { get; set; }
        private readonly TranslationCheckbox _translationCheckbox;

        public ShopEditorPage()
        {
            Panel wrapper = new(Controls);

            TranslationDropdown roleDropdown = wrapper.Add.TranslationDropdown();
            roleDropdown.AddTooltip(new TranslationData("MENU.SHOPEDITOR.SELECTROLE"), "roleselection");
            roleDropdown.AddEventListener("onchange", () =>
            {
                bool hasRoleSelected = roleDropdown.Selected != null && roleDropdown.Selected.Value is Role role;
                _translationCheckbox.SetClass("inactive", !hasRoleSelected);

                if (hasRoleSelected)
                {
                    CreateRoleShopContent(roleDropdown.Selected.Value as Role);
                }
            });

            foreach (Type roleType in Utils.GetTypes<Role>())
            {
                Role role = Utils.GetObjectByType<Role>(roleType);

                if (role == null)
                {
                    continue;
                }

                TranslationOption option = new(new TranslationData(role.GetTranslationKey("NAME")), role);
                roleDropdown.Options.Add(option);
                roleDropdown.Select(option);
            }

            wrapper.Add.HorizontalLineBreak();

            _translationCheckbox = wrapper.Add.TranslationCheckbox(new TranslationData("MENU.SHOPEDITOR.ENABLEROLE"));
            _translationCheckbox.AddClass("inactive");
            _translationCheckbox.AddEventListener("onchange", () =>
            {
                if (roleDropdown.Selected.Value is Role role)
                {
                    ServerToggleShop(role.Name, _translationCheckbox.Checked);
                }
            });
        }

        private void CreateRoleShopContent(Role selectedRole)
        {
            RoleShopContent.DeleteChildren(true);
            ShopItems.Clear();

            _translationCheckbox.Checked = selectedRole.Shop.Enabled;

            foreach (Type itemType in Utils.GetTypesWithAttribute<IItem, BuyableAttribute>())
            {
                ShopItemData shopItemData = ShopItemData.CreateItemData(itemType);

                if (shopItemData == null)
                {
                    continue;
                }

                Panel wrapper = new(RoleShopContent);
                wrapper.AddClass("row");

                QuickShopItem item = new();
                item.SetItem(shopItemData);
                item.AddEventListener("onclick", () =>
                {
                    ToggleItem(item, selectedRole);
                });

                foreach (ShopItemData loopItemData in selectedRole.Shop.Items)
                {
                    if (loopItemData.Name.Equals(shopItemData.Name))
                    {
                        shopItemData.CopyFrom(loopItemData);

                        item.SetItem(shopItemData);
                        item.SetClass("selected", true);
                    }
                }

                wrapper.Add.HorizontalLineBreak();

                wrapper.Add.TranslationButton(new TranslationData(), "settings", null, () =>
                {
                    EditItem(item, selectedRole);
                });

                wrapper.Add.LineBreak();

                ShopItems.Add(item);
                wrapper.AddChild(item);
            }
        }
    }
}
