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
        private Panel Controls { get; set; }
        private Panel RoleShopContent { get; set; }
        private readonly TranslationCheckbox _translationCheckbox;

        private readonly List<QuickShopItem> _shopItems = new();

        public ShopEditorPage()
        {
            Panel wrapper = new(Controls);

            TranslationDropdown roleDropdown = wrapper.Add.TranslationDropdown();
            roleDropdown.AddTooltip(new TranslationData("MENU_SHOPEDITOR_SELECTROLE"), "roleselection");
            roleDropdown.AddEventListener("onchange", () =>
            {
                bool hasRoleSelected = roleDropdown.Selected != null && roleDropdown.Selected.Value is TTTRole role;
                _translationCheckbox.SetClass("inactive", !hasRoleSelected);

                if (hasRoleSelected)
                {
                    CreateRoleShopContent(roleDropdown.Selected.Value as TTTRole);
                }
            });

            foreach (Type roleType in Utils.GetTypes<TTTRole>())
            {
                TTTRole role = Utils.GetObjectByType<TTTRole>(roleType);

                if (role == null)
                {
                    continue;
                }

                TranslationOption option = new(new TranslationData(role.GetRoleTranslationKey("NAME")), role);
                roleDropdown.Options.Add(option);
                roleDropdown.Select(option);
            }

            wrapper.Add.HorizontalLineBreak();

            _translationCheckbox = wrapper.Add.TranslationCheckbox(new TranslationData("MENU_SHOPEDITOR_ENABLEROLE"));
            _translationCheckbox.AddClass("inactive");
            _translationCheckbox.AddEventListener("onchange", () =>
            {
                if (roleDropdown.Selected.Value is TTTRole role)
                {
                    ServerToggleShop(role.Name, _translationCheckbox.Checked);
                }
            });
        }

        private void CreateRoleShopContent(TTTRole selectedRole)
        {
            RoleShopContent.DeleteChildren(true);
            _shopItems.Clear();

            _translationCheckbox.Checked = selectedRole.Shop.Enabled;

            foreach (Type itemType in Utils.GetTypesWithAttribute<IItem, BuyableAttribute>())
            {
                ShopItemData shopItemData = ShopItemData.CreateItemData(itemType);

                if (shopItemData == null)
                {
                    continue;
                }

                QuickShopItem item = new(RoleShopContent);
                item.SetItem(shopItemData);

                // ON CLICK LEFT CLICK PLUS RIGHT CLICK

                foreach (ShopItemData loopItemData in selectedRole.Shop.Items)
                {
                    if (loopItemData.Name.Equals(shopItemData.Name))
                    {
                        shopItemData.CopyFrom(loopItemData);

                        item.SetItem(shopItemData);
                        item.SetClass("selected", true);
                    }
                }
            }
        }
    }
}