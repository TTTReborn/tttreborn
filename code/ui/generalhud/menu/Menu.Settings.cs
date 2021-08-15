using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

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
                    tabs.AddTab("Server", CreateServerSettings);
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

        private void CreateServerSettings(PanelContent menuContent)
        {
            menuContent.Add.Label("Work in progress...");
        }
    }
}
