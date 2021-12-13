using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Roles;
using TTTReborn.UI.Menu;

namespace TTTReborn.UI
{
    [UseTemplate]
    public class ShopEditorPage : Panel
    {
        private TranslationCheckbox _translationCheckbox;

        public ShopEditorPage()
        {
            Panel wrapper = new(this);

            _translationCheckbox = wrapper.Add.TranslationCheckbox(new TranslationData("MENU_SHOPEDITOR_ENABLEROLE"));
            _translationCheckbox.AddClass("inactive");

            wrapper.Add.HorizontalLineBreak();

            TranslationDropdown roleDropdown = wrapper.Add.TranslationDropdown();
            roleDropdown.AddTooltip(new TranslationData("MENU_SHOPEDITOR_SELECTROLE"), "roleselection");

            roleDropdown.AddEventListener("onchange", () =>
            {
                bool hasRoleSelected = roleDropdown.Selected != null && roleDropdown.Selected.Value is TTTRole role;
                _translationCheckbox.SetClass("inactive", !hasRoleSelected);

                if (hasRoleSelected)
                {

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
        }

        private void CreateRoleShopContent(TTTRole selectedRole)
        {

        }
    }
}