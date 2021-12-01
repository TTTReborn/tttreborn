using System.Collections.Generic;

using Sandbox;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void OpenKeybindings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.TranslationLabel(new Globalization.TranslationData("MENU_KEYBINDINGS_DESCRIPTION"));
                panelContent.Add.Label("");

                panelContent.Add.TranslationLabel(new Globalization.TranslationData("MENU_KEYBINDINGS_MOVEMENT"), "binding-header");
                CreateBinding(menuContent, "MENU_KEYBINDINGS_FORWARD", new() { "+iv_forward" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_BACK", new() { "+iv_back" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_LEFT", new() { "+iv_left" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_RIGHT", new() { "+iv_right" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_JUMP", new() { "+iv_jump" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_CROUCH", new() { "+iv_duck" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_SPRINT", new() { "+iv_sprint" });
                panelContent.Add.Label("");

                panelContent.Add.TranslationLabel(new Globalization.TranslationData("MENU_KEYBINDINGS_WEAPONS"), "binding-header");
                CreateBinding(menuContent, "MENU_KEYBINDINGS_FIRE", new() { "+iv_attack" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_RELOAD", new() { "+iv_reload" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_DROP_WEAPON", new() { "+iv_drop" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_DROP_AMMO", new() { "+iv_sprint", "+iv_drop" });
                panelContent.Add.Label("");

                panelContent.Add.TranslationLabel(new Globalization.TranslationData("MENU_KEYBINDINGS_ACTIONS"), "binding-header");
                CreateBinding(menuContent, "MENU_KEYBINDINGS_USE", new() { "+iv_use" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_FLASHLIGHT", new() { "+iv_flashlight" });
                panelContent.Add.Label("");

                panelContent.Add.TranslationLabel(new Globalization.TranslationData("MENU_KEYBINDINGS_COMMUNICATION"), "binding-header");
                CreateBinding(menuContent, "MENU_KEYBINDINGS_VOICE_CHAT", new() { "+iv_voice" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_TEAM_VOICE_CHAT", new() { "+iv_walk" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_TEAM_TEXT_CHAT", new() { "+iv_score" });
                panelContent.Add.Label("");

                panelContent.Add.TranslationLabel(new Globalization.TranslationData("MENU_KEYBINDINGS_MENUS"), "binding-header");
                CreateBinding(menuContent, "MENU_KEYBINDINGS_SCOREBOARD", new() { "+iv_score" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_MENU", new() { "+iv_menu" });
                CreateBinding(menuContent, "MENU_KEYBINDINGS_QUICK_SHOP", new() { "+iv_view" });
                panelContent.Add.Label("");

            }, "MENU_KEYBINDINGS", "keybindings");
        }

        private void CreateBinding(PanelContent menuContent, string actionName, List<string> bindings)
        {
            Panel wrapper = new(menuContent);
            wrapper.AddClass("wrapper");

            wrapper.Add.TranslationLabel(new Globalization.TranslationData(actionName));
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
