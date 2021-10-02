using Sandbox;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    using Sandbox.UI.Construct;

    using TTTReborn.Globals;

    public partial class Menu
    {
        public PanelContent ServerSettingsTabContent { get; private set; }

        public Tabs SettingsTabs { get; private set; }

        internal void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((menuContent) =>
            {
                SettingsTabs = menuContent.Add.Tabs();
                SettingsTabs.AddTab("Client", CreateClientSettings, Utils.Realm.Client);

                if (Local.Client.HasPermission("serversettings"))
                {
                    SettingsTabs.AddTab("Server", InitServerSettings, Utils.Realm.Server);
                }

                CreateSettingsButtons(menuContent);
            }, "Settings", "settings");
        }

        private void InitServerSettings(PanelContent tabContent)
        {
            ServerSettingsTabContent = tabContent;

            tabContent.Add.Label("Loading...");

            SettingFunctions.RequestServerSettings();
        }

        internal void ProceedServerSettings(ServerSettings serverSettings)
        {
            if (!Enabled || ServerSettingsTabContent == null)
            {
                return;
            }

            ServerSettingsTabContent.SetPanelContent((menuContent) => CreateServerSettings(menuContent, serverSettings));
        }
    }
}
