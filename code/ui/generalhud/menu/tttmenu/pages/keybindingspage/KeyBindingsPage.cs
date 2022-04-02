using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.UI;

namespace TTTReborn.UI.Menu
{
    public partial class KeyBindingsPage : Panel
    {
        public KeyBindingsPage()
        {
            StyleSheet.Load("/ui/generalhud/menu/tttmenu/pages/KeyBindingsPage/KeyBindingsPage.scss");

            Add.TranslationLabel(new TranslationData("MENU.KEYBINDINGS.RESTRICTION"));
            Add.Label("");

            TranslationTabContainer translationTabContainer = Add.TranslationTabContainer();

            Panel movementPanel = new();
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.FORWARD", InputButton.Forward));
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.BACK", InputButton.Back));
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.LEFT", InputButton.Left));
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.RIGHT", InputButton.Right));
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.JUMP", InputButton.Jump));
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.CROUCH", InputButton.Duck));
            movementPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MOVEMENT.SPRINT", InputButton.Run));

            translationTabContainer.AddTab(movementPanel, new TranslationData("MENU.KEYBINDINGS.MOVEMENT.TITLE"));

            Panel weaponsPanel = new();
            weaponsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.WEAPONS.FIRE", InputButton.Attack1));
            weaponsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.WEAPONS.RELOAD", InputButton.Reload));
            weaponsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.WEAPONS.DROP_WEAPON", InputButton.Drop));
            weaponsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.WEAPONS.DROP_AMMO", InputButton.Run, InputButton.Drop));

            translationTabContainer.AddTab(weaponsPanel, new TranslationData("MENU.KEYBINDINGS.WEAPONS.TITLE"));

            Panel actionsPanel = new();
            actionsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.ACTIONS.USE", InputButton.Use));
            actionsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.ACTIONS.FLASHLIGHT", InputButton.Flashlight));

            translationTabContainer.AddTab(actionsPanel, new TranslationData("MENU.KEYBINDINGS.ACTIONS.TITLE"));

            Panel communicationsPanel = new();
            communicationsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.COMMUNICATION.VOICECHAT", InputButton.Voice));
            communicationsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.COMMUNICATION.TEAMVOICECHAT", InputButton.Walk));
            communicationsPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.COMMUNICATION.TEAMTEXTCHAT", InputButton.Score));

            translationTabContainer.AddTab(communicationsPanel, new TranslationData("MENU.KEYBINDINGS.COMMUNICATION.TITLE"));

            Panel menusPanel = new();
            menusPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MENUS.SCOREBOARD", InputButton.Score));
            menusPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MENUS.MENU", InputButton.Menu));
            menusPanel.AddChild(new BindingPanel("MENU.KEYBINDINGS.MENUS.QUICKSHOP", InputButton.View));

            translationTabContainer.AddTab(menusPanel, new TranslationData("MENU.KEYBINDINGS.MENUS.TITLE"));
        }
    }

    public class BindingPanel : Panel
    {
        public string TranslationKey { get; set; }
        public InputButton[] InputButtons { get; set; }

        public BindingPanel(string translationKey, params InputButton[] inputButtons) : base()
        {
            TranslationKey = translationKey;
            InputButtons = inputButtons;

            AddClass("wrapper");
            Add.TranslationLabel(new TranslationData(translationKey));
            Add.Label(": ");

            for (int i = 0; i < inputButtons.Length; ++i)
            {
                AddChild(new BindingKeyImage(inputButtons[i]));
                Add.Label($" ({inputButtons[i]}) ");

                // Don't show a + if it's the last binding in the list.
                if (i != inputButtons.Length - 1)
                {
                    Add.Label(" + ", "text-color-info");
                }
            }
        }
    }
}
