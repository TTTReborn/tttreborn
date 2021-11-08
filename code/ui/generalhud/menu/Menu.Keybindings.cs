using System.Collections.Generic;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void OpenKeybindings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                panelContent.Add.Label("You can change your bindings in the s&box options menu or through console.");
                panelContent.Add.Label("");

                panelContent.Add.Label("Movement");
                CreateBinding(menuContent, "Forward", new List<string> { "+iv_forward" });
                CreateBinding(menuContent, "Back", new List<string> { "+iv_back" });
                CreateBinding(menuContent, "Left", new List<string> { "+iv_left" });
                CreateBinding(menuContent, "Right", new List<string> { "+iv_right" });
                CreateBinding(menuContent, "Jump", new List<string> { "+iv_jump" });
                CreateBinding(menuContent, "Crouch", new List<string> { "+iv_duck" });
                CreateBinding(menuContent, "Sprint", new List<string> { "+iv_sprint" });
                panelContent.Add.Label("");

                panelContent.Add.Label("Weapons");
                CreateBinding(menuContent, "Fire", new List<string> { "+iv_attack" });
                CreateBinding(menuContent, "Reloaded", new List<string> { "+iv_reload" });
                CreateBinding(menuContent, "Drop Weapon", new List<string> { "+iv_drop" });
                CreateBinding(menuContent, "Drop Ammo", new List<string> { "+iv_sprint", "+iv_drop" });
                panelContent.Add.Label("");

                panelContent.Add.Label("Actions");
                CreateBinding(menuContent, "Use", new List<string> { "+iv_use" });
                CreateBinding(menuContent, "Flashlight", new List<string> { "+iv_flashlight" });
                panelContent.Add.Label("");

                panelContent.Add.Label("Communication");
                CreateBinding(menuContent, "Voice Chat", new List<string> { "+iv_voice" });
                CreateBinding(menuContent, "Team Voice Chat", new List<string> { "+iv_walk" });
                CreateBinding(menuContent, "Team Text Chat", new List<string> { "+iv_score" });
                panelContent.Add.Label("");

                panelContent.Add.Label("Menus");
                CreateBinding(menuContent, "Scoreboard", new List<string> { "+iv_score" });
                CreateBinding(menuContent, "Menu", new List<string> { "+iv_menu" });
                CreateBinding(menuContent, "Quick Shop", new List<string> { "+iv_view" });
                panelContent.Add.Label("");

            }, "MENU_KEYBINDINGS", "keybindings");
        }

        private void CreateBinding(PanelContent menuContent, string action, List<string> bindings)
        {
            Panel wrapper = new(menuContent);
            wrapper.AddClass("wrapper");

            Label actionLabel = wrapper.Add.Label($"{action}: ");
            actionLabel.Style.Width = 150;

            for (int i = 0; i < bindings.Count; ++i)
            {
                string binding = bindings[i];
                wrapper.Add.Label($"{Input.GetKeyWithBinding(binding).ToUpper()}", "text-color-info");
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
