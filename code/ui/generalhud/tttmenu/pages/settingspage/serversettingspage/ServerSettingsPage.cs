using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ServerSettingsPage : Panel
    {
        private TranslationTabContainer TabContainer { get; set; }
        private readonly FileSelection _currentFileSelection;

        public ServerSettingsPage(ServerSettings serverSettings)
        {
            SettingsPage.CreateSettings(TabContainer, serverSettings);
            SettingsPage.CreateFileSelectionButtons(this, _currentFileSelection, true);
        }
    }
}
