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

            Add.TranslationLabel(new TranslationData("MENU_KEYBINDINGS_DESCRIPTION"));
            Add.Label("");

            TranslationTabContainer translationTabContainer = Add.TranslationTabContainer();

            Panel movementPanel = new();
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_FORWARD", new() { "+iv_forward" });
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_BACK", new() { "+iv_back" });
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_LEFT", new() { "+iv_left" });
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_RIGHT", new() { "+iv_right" });
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_JUMP", new() { "+iv_jump" });
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_CROUCH", new() { "+iv_duck" });
            CreateBinding(movementPanel, "MENU_KEYBINDINGS_SPRINT", new() { "+iv_sprint" });

            translationTabContainer.AddTab(movementPanel, new TranslationData("MENU_KEYBINDINGS_MOVEMENT"));

            Panel weaponsPanel = new();
            CreateBinding(weaponsPanel, "MENU_KEYBINDINGS_FIRE", new() { "+iv_attack" });
            CreateBinding(weaponsPanel, "MENU_KEYBINDINGS_RELOAD", new() { "+iv_reload" });
            CreateBinding(weaponsPanel, "MENU_KEYBINDINGS_DROP_WEAPON", new() { "+iv_drop" });
            CreateBinding(weaponsPanel, "MENU_KEYBINDINGS_DROP_AMMO", new() { "+iv_sprint", "+iv_drop" });

            translationTabContainer.AddTab(weaponsPanel, new TranslationData("MENU_KEYBINDINGS_WEAPONS"));

            Panel actionsPanel = new();
            CreateBinding(actionsPanel, "MENU_KEYBINDINGS_USE", new() { "+iv_use" });
            CreateBinding(actionsPanel, "MENU_KEYBINDINGS_FLASHLIGHT", new() { "+iv_flashlight" });

            translationTabContainer.AddTab(actionsPanel, new TranslationData("MENU_KEYBINDINGS_ACTIONS"));

            Panel communicationsPanel = new();
            CreateBinding(communicationsPanel, "MENU_KEYBINDINGS_VOICE_CHAT", new() { "+iv_voice" });
            CreateBinding(communicationsPanel, "MENU_KEYBINDINGS_TEAM_VOICE_CHAT", new() { "+iv_walk" });
            CreateBinding(communicationsPanel, "MENU_KEYBINDINGS_TEAM_TEXT_CHAT", new() { "+iv_score" });

            translationTabContainer.AddTab(communicationsPanel, new TranslationData("MENU_KEYBINDINGS_COMMUNICATION"));

            Panel menusPanel = new();
            CreateBinding(menusPanel, "MENU_KEYBINDINGS_SCOREBOARD", new() { "+iv_score" });
            CreateBinding(menusPanel, "MENU_KEYBINDINGS_MENU", new() { "+iv_menu" });
            CreateBinding(menusPanel, "MENU_KEYBINDINGS_QUICK_SHOP", new() { "+iv_view" });

            translationTabContainer.AddTab(menusPanel, new TranslationData("MENU_KEYBINDINGS_MENUS"));
        }

        private static void CreateBinding(Panel parent, string actionName, List<string> bindings)
        {
            Panel wrapper = parent.Add.Panel("wrapper");
            wrapper.Add.TranslationLabel(new TranslationData(actionName));
            wrapper.Add.Label(": ");

            for (int i = 0; i < bindings.Count; ++i)
            {
                string binding = bindings[i];
                wrapper.Add.Label(Input.GetKeyWithBinding(binding).ToUpper(), "text-color-info");
                wrapper.Add.Label($" ({binding}) ");

                // Don't show a + if it's the last binding in the list.
                if (i != bindings.Count - 1)
                {
                    wrapper.Add.Label(" + ", "text-color-info");
                }
            }
        }
    }
}
