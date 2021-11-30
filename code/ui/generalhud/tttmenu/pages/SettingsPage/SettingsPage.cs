using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    [UseTemplate]
    public partial class SettingsPage : Panel
    {
        private TranslationButton ServerSettingsButton { get; set; }

        public SettingsPage()
        {
            if (Local.Client.HasPermission("serversettings"))
            {
                ServerSettingsButton.RemoveClass("inactive");
            }
        }

        public void GoToClientSettings()
        {
            TTTMenu.Instance.AddPage(new ClientSettingsPage());
        }

        public void GoToServerSettings()
        {
            Settings.SettingFunctions.FetchAndOpenServerSettingsPage();
        }

        public static void CreateSwitchSetting(Panel parent, Settings.Settings settings, string categoryName, string propertyName, object propertyObject)
        {
            TranslationCheckbox checkbox = parent.Add.TranslationCheckbox($"MENU_SETTINGS_{categoryName.ToUpper()}_{propertyName.ToUpper()}");
        }
    }
}
