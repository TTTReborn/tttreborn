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

            ConsoleSystem.Run("ttt_serversettings_request");
        }
    }
}

namespace TTTReborn.Player
{
    using TTTReborn.UI.Menu;

    public partial class TTTPlayer
    {
        [ServerCmd(Name = "ttt_serversettings_request")]
        public static void RequestServerSettings()
        {
            if (!ConsoleSystem.Caller.HasPermission("serversettings"))
            {
                return;
            }

            ClientSendServerSettings(To.Single(ConsoleSystem.Caller), SettingFunctions.GetJSON<ServerSettings>(ServerSettings.Instance, true));
        }

        [ServerCmd(Name = "ttt_serversettings_send")]
        public static void SendServerSettings(string serverSettingsJson)
        {
            if (!ConsoleSystem.Caller.HasPermission("serversettings"))
            {
                return;
            }

            ServerSettings serverSettings = SettingFunctions.GetSettings<ServerSettings>(serverSettingsJson);

            if (serverSettings == null)
            {
                return;
            }

            SettingsManager.Instance = serverSettings;

            SettingFunctions.SaveSettings<ServerSettings>(ServerSettings.Instance);
        }

        [ClientRpc]
        public static void ClientSendServerSettings(string serverSettingsJson)
        {
            Menu menu = Menu.Instance;

            if (menu == null || !menu.IsShowing || menu.ServerSettingsTabContent == null)
            {
                return;
            }

            ServerSettings serverSettings = SettingFunctions.GetSettings<ServerSettings>(serverSettingsJson);

            if (serverSettings == null)
            {
                return;
            }

            menu.ServerSettingsTabContent.SetPanelContent((menuContent) => menu.CreateServerSettings(menuContent, serverSettings));
        }
    }
}
