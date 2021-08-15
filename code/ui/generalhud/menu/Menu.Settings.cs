using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        internal void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((menuContent) =>
            {
                Tabs tabs = menuContent.Add.Tabs();
                tabs.AddTab("Client", CreateClientSettings);

                if (Local.Client.HasPermission("serversettings"))
                {
                    tabs.AddTab("Server", InitServerSettings);
                }

                CreateSettingsButtons(menuContent);
            }, "Settings", "settings");
        }

        private void CreateClientSettings(PanelContent menuContent)
        {
            Panel languagePanel = menuContent.Add.Panel("language");

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

        private void InitServerSettings(PanelContent menuContent)
        {
            menuContent.Add.Label("Loading...");

            ConsoleSystem.Run("ttt_serversettings_request");
        }

        internal void CreateServerSettings(PanelContent menuContent, ServerSettings serverSettings)
        {
            menuContent.DeleteChildren(true);

            menuContent.Add.Label($"Sprint enabled? {serverSettings.IsSprintEnabled.ToString()}");
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

            ClientSendServerSettings(To.Single(ConsoleSystem.Caller), Settings.SettingFunctions.GetJSON<ServerSettings>(SettingsManager.Instance as ServerSettings));
        }

        [ClientRpc]
        public static void ClientSendServerSettings(string serverSettingsJson)
        {
            Menu menu = Menu.Instance;

            if (menu == null || !menu.IsShowing)
            {
                return;
            }

            ServerSettings serverSettings = Settings.SettingFunctions.GetSettings<ServerSettings>(serverSettingsJson);

            if (serverSettingsJson == null)
            {
                return;
            }

            menu.MenuContent.SetPanelContent((menuContent) => menu.CreateServerSettings(menuContent, serverSettings));
        }
    }
}
