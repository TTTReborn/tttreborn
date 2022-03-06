using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI.Menu
{
    public partial class KeyBindingsPage : Panel
    {
        public KeyBindingsPage()
        {
            StyleSheet.Load("/ui/generalhud/tttmenu/pages/KeyBindingsPage/KeyBindingsPage.scss");

            Add.TranslationLabel(new TranslationData("MENU.KEYBINDINGS.RESTRICTION"));
            Add.Label("");

            TranslationTabContainer translationTabContainer = Add.TranslationTabContainer();

            Panel movementPanel = new();
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.FORWARD", "+iv_forward");
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.BACK", "+iv_back");
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.LEFT", "+iv_left");
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.RIGHT", "+iv_right");
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.JUMP", "+iv_jump");
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.CROUCH", "+iv_duck");
            CreateBinding(movementPanel, "MENU.KEYBINDINGS.MOVEMENT.SPRINT", "+iv_sprint");

            translationTabContainer.AddTab(movementPanel, new TranslationData("MENU.KEYBINDINGS.MOVEMENT.TITLE"));

            Panel weaponsPanel = new();
            CreateBinding(weaponsPanel, "MENU.KEYBINDINGS.WEAPONS.FIRE", "+iv_attack");
            CreateBinding(weaponsPanel, "MENU.KEYBINDINGS.WEAPONS.RELOAD", "+iv_reload");
            CreateBinding(weaponsPanel, "MENU.KEYBINDINGS.WEAPONS.DROP_WEAPON", "+iv_drop");
            CreateBinding(weaponsPanel, "MENU.KEYBINDINGS.WEAPONS.DROP_AMMO", "+iv_sprint", "+iv_drop");

            translationTabContainer.AddTab(weaponsPanel, new TranslationData("MENU.KEYBINDINGS.WEAPONS.TITLE"));

            Panel actionsPanel = new();
            CreateBinding(actionsPanel, "MENU.KEYBINDINGS.ACTIONS.USE", "+iv_use");
            CreateBinding(actionsPanel, "MENU.KEYBINDINGS.ACTIONS.FLASHLIGHT", "+iv_flashlight");

            translationTabContainer.AddTab(actionsPanel, new TranslationData("MENU.KEYBINDINGS.ACTIONS.TITLE"));

            Panel communicationsPanel = new();
            CreateBinding(communicationsPanel, "MENU.KEYBINDINGS.COMMUNICATION.VOICECHAT", "+iv_voice");
            CreateBinding(communicationsPanel, "MENU.KEYBINDINGS.COMMUNICATION.TEAMVOICECHAT", "+iv_walk");
            CreateBinding(communicationsPanel, "MENU.KEYBINDINGS.COMMUNICATION.TEAMTEXTCHAT", "+iv_score");

            translationTabContainer.AddTab(communicationsPanel, new TranslationData("MENU.KEYBINDINGS.COMMUNICATION.TITLE"));

            Panel menusPanel = new();
            CreateBinding(menusPanel, "MENU.KEYBINDINGS.MENUS.SCOREBOARD", "+iv_score");
            CreateBinding(menusPanel, "MENU.KEYBINDINGS.MENUS.MENU", "+iv_menu");
            CreateBinding(menusPanel, "MENU.KEYBINDINGS.MENUS.QUICKSHOP", "+iv_view");

            translationTabContainer.AddTab(menusPanel, new TranslationData("MENU.KEYBINDINGS.MENUS.TITLE"));
        }

        private static void CreateBinding(Panel parent, string actionName, params string[] bindings)
        {
            Panel wrapper = parent.Add.Panel("wrapper");
            wrapper.Add.TranslationLabel(new TranslationData(actionName));
            wrapper.Add.Label(": ");

            for (int i = 0; i < bindings.Length; ++i)
            {
                string binding = bindings[i];
                wrapper.Add.Label(Input.GetKeyWithBinding(binding).ToUpper(), "text-color-info");
                wrapper.Add.Label($" ({binding}) ");

                // Don't show a + if it's the last binding in the list.
                if (i != bindings.Length - 1)
                {
                    wrapper.Add.Label(" + ", "text-color-info");
                }
            }
        }
    }
}
