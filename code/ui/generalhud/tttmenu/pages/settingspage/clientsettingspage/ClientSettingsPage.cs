using Sandbox;
using Sandbox.UI;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class ClientSettingsPage : Panel
    {
        private TranslationTabContainer TabContainer { get; set; }
        private Panel Buttons { get; set; }
        private readonly FileSelection _currentFileSelection;


        public ClientSettingsPage()
        {
            SettingsPage.CreateSettings(TabContainer, ClientSettings.Instance);
            SettingsPage.CreateFileSelectionButtons(Buttons, _currentFileSelection, false);
        }
    }
}
