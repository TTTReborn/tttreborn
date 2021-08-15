using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                Tabs tabs = panelContent.Add.Tabs();
                tabs.AddTab("Client", CreateClientSettings);

                if (Local.Client.HasPermission("serversettings"))
                {
                    tabs.AddTab("Server", CreateServerSettings);
                }

                CreateSettingsButtons(panelContent);
            }, "Settings", "settings");
        }

        private void CreateClientSettings(PanelContent panelContent)
        {
            Panel languagePanel = panelContent.Add.Panel("language");

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

        private void CreateServerSettings(PanelContent panelContent)
        {
            panelContent.Add.Label("Work in progress...");
        }
    }
}
