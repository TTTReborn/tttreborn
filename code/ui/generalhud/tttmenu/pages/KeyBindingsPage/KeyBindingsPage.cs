using System.Collections.Generic;

using Sandbox;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class KeyBindingsPage : Panel
    {
        public KeyBindingsPage()
        {
            StyleSheet.Load("/ui/generalhud/tttmenu/pages/KeyBindingsPage/KeyBindingsPage.scss");

            Add.TranslationLabel("MENU_KEYBINDINGS_DESCRIPTION");
            Add.Label("");

            Add.TranslationLabel("MENU_KEYBINDINGS_MOVEMENT", "binding-header");
            CreateBinding(this, "MENU_KEYBINDINGS_FORWARD", new() { "+iv_forward" });
            CreateBinding(this, "MENU_KEYBINDINGS_BACK", new() { "+iv_back" });
            CreateBinding(this, "MENU_KEYBINDINGS_LEFT", new() { "+iv_left" });
            CreateBinding(this, "MENU_KEYBINDINGS_RIGHT", new() { "+iv_right" });
            CreateBinding(this, "MENU_KEYBINDINGS_JUMP", new() { "+iv_jump" });
            CreateBinding(this, "MENU_KEYBINDINGS_CROUCH", new() { "+iv_duck" });
            CreateBinding(this, "MENU_KEYBINDINGS_SPRINT", new() { "+iv_sprint" });
            Add.Label("");

            Add.TranslationLabel("MENU_KEYBINDINGS_WEAPONS", "binding-header");
            CreateBinding(this, "MENU_KEYBINDINGS_FIRE", new() { "+iv_attack" });
            CreateBinding(this, "MENU_KEYBINDINGS_RELOAD", new() { "+iv_reload" });
            CreateBinding(this, "MENU_KEYBINDINGS_DROP_WEAPON", new() { "+iv_drop" });
            CreateBinding(this, "MENU_KEYBINDINGS_DROP_AMMO", new() { "+iv_sprint", "+iv_drop" });
            Add.Label("");

            Add.TranslationLabel("MENU_KEYBINDINGS_ACTIONS", "binding-header");
            CreateBinding(this, "MENU_KEYBINDINGS_USE", new() { "+iv_use" });
            CreateBinding(this, "MENU_KEYBINDINGS_FLASHLIGHT", new() { "+iv_flashlight" });
            Add.Label("");

            Add.TranslationLabel("MENU_KEYBINDINGS_COMMUNICATION", "binding-header");
            CreateBinding(this, "MENU_KEYBINDINGS_VOICE_CHAT", new() { "+iv_voice" });
            CreateBinding(this, "MENU_KEYBINDINGS_TEAM_VOICE_CHAT", new() { "+iv_walk" });
            CreateBinding(this, "MENU_KEYBINDINGS_TEAM_TEXT_CHAT", new() { "+iv_score" });
            Add.Label("");

            Add.TranslationLabel("MENU_KEYBINDINGS_MENUS", "binding-header");
            CreateBinding(this, "MENU_KEYBINDINGS_SCOREBOARD", new() { "+iv_score" });
            CreateBinding(this, "MENU_KEYBINDINGS_MENU", new() { "+iv_menu" });
            CreateBinding(this, "MENU_KEYBINDINGS_QUICK_SHOP", new() { "+iv_view" });
            Add.Label("");
        }

        private static void CreateBinding(Panel parent, string action, List<string> bindings)
        {
            Panel wrapper = new(parent);
            wrapper.AddClass("wrapper");

            wrapper.Add.TranslationLabel(action);
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
