using System.Collections.Generic;

using Sandbox;
using Sandbox.UI.Construct;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class SettingsPage : Panel
    {
        public PanelContent ServerSettingsTabContent { get; private set; }

        public Tabs SettingsTabs { get; private set; }

        public SettingsPage()
        {
            StyleSheet.Load("/ui/generalhud/tttmenu/pages/SettingsPage/SettingsPage.scss");

            SettingsTabs = Add.Tabs();
            SettingsTabs.AddTab("REALM_CLIENT", (panelContent) =>
            {
                panelContent.SetPanelContent((content) => CreateSettings(content.Add.Tabs(), Settings.ClientSettings.Instance));
            }, Utils.Realm.Client);

            if (Local.Client.HasPermission("serversettings"))
            {
                SettingsTabs.AddTab("REALM_SERVER", InitServerSettings, Utils.Realm.Server);
            }

            CreateSettingsButtons();
        }

        public void ProceedServerSettings(ServerSettings serverSettings)
        {
            if (!Enabled || ServerSettingsTabContent == null)
            {
                return;
            }

            ServerSettingsTabContent.SetPanelContent((menuContent) => CreateSettings(menuContent.Add.Tabs(), serverSettings));
        }

        private void InitServerSettings(PanelContent tabContent)
        {
            ServerSettingsTabContent = tabContent;

            tabContent.Add.TranslationLabel("MENU_SETTINGS_LOADING");

            SettingFunctions.RequestServerSettings();
        }
    }
}
