using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ServerSettingsPage : Panel
    {
        private TranslationTabContainer TabContainer { get; set; }
        private Panel Buttons { get; set; }

        public ServerSettingsPage(ServerSettings serverSettings)
        {
            SettingsPage.CreateSettings(TabContainer, serverSettings);
            SettingsPage.CreateFileSelectionButtons(Buttons, true);
        }
    }
}
