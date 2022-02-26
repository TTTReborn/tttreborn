using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ClientSettingsPage : Panel
    {
        private TranslationTabContainer TabContainer { get; set; }
        private Panel Buttons { get; set; }

        public ClientSettingsPage()
        {
            SettingsPage.CreateSettings(TabContainer, ClientSettings.Instance);
            SettingsPage.CreateFileSelectionButtons(Buttons, false);
        }
    }
}
