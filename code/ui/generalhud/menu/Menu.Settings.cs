using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Globalization;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
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

        private void CreateClientSettings(PanelContent tabContent)
        {
            Panel languagePanel = tabContent.Add.Panel("language");

            languagePanel.Add.Label("Language:");

            Dropdown languageSelection = languagePanel.Add.Dropdown();

            foreach (Language language in TTTLanguage.Languages.Values)
            {
                languageSelection.AddOption(language.Data.Name, language.Data.Code);
            }

            languageSelection.OnSelectOption = (option) =>
            {
                ConsoleSystem.Run("ttt_language", (string) option.Data);
            };

            languageSelection.SelectByData(Settings.SettingsManager.Instance.Language);
        }

        private void InitServerSettings(PanelContent tabContent)
        {
            ServerSettingsTabContent = tabContent;

            tabContent.Add.Label("Loading...");

            ConsoleSystem.Run("ttt_serversettings_request");
        }

        internal void CreateServerSettings(PanelContent tabContent, ServerSettings serverSettings)
        {
            tabContent.DeleteChildren(true);

            tabContent.Add.Label($"Sprint enabled?");
            Switch sw = tabContent.Add.Switch("sprint", serverSettings.IsSprintEnabled);

            // TODO remove later due to avoid tons of syncing
            sw.AddEventListener("onchange", (panelEvent) =>
            {
                serverSettings.IsSprintEnabled = !serverSettings.IsSprintEnabled;

                ConsoleSystem.Run("ttt_serversettings_send", Settings.SettingFunctions.GetJSON<ServerSettings>(serverSettings, true));
            });

            // TODO add save button to sync settings back to the server
            // TODO reuse save as button
            // TODO reuse load from button
            // TODO update server settings for other admins that have the settings opened -> needed? Discuss!
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

            ClientSendServerSettings(To.Single(ConsoleSystem.Caller), Settings.SettingFunctions.GetJSON<ServerSettings>(SettingsManager.Instance as ServerSettings, true));
        }

        [ServerCmd(Name = "ttt_serversettings_send")]
        public static void SendServerSettings(string serverSettingsJson)
        {
            if (!ConsoleSystem.Caller.HasPermission("serversettings"))
            {
                return;
            }

            ServerSettings serverSettings = Settings.SettingFunctions.GetSettings<ServerSettings>(serverSettingsJson);

            if (serverSettingsJson == null)
            {
                return;
            }

            Settings.SettingsManager.Instance = serverSettings;
        }

        [ClientRpc]
        public static void ClientSendServerSettings(string serverSettingsJson)
        {
            Menu menu = Menu.Instance;

            if (menu == null || !menu.IsShowing || menu.ServerSettingsTabContent == null)
            {
                return;
            }

            ServerSettings serverSettings = Settings.SettingFunctions.GetSettings<ServerSettings>(serverSettingsJson);

            if (serverSettingsJson == null)
            {
                return;
            }

            menu.ServerSettingsTabContent.SetPanelContent((menuContent) => menu.CreateServerSettings(menuContent, serverSettings));
        }
    }
}
