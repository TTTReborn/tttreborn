using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ServerSettingsPage : Panel
    {
        private TranslationTabContainer TabContainer { get; set; }

        private FileSelection ServerSettingsFileSelection { get; set; }
        private FileSelection _currentFileSelection;

        public ServerSettingsPage(ServerSettings serverSettings)
        {
            SettingsPage.CreateSettings(TabContainer, serverSettings);
            SettingsPage.CreateFileSelectionButtons(this, _currentFileSelection, ServerSettingsFileSelection, true);
        }
    }
}