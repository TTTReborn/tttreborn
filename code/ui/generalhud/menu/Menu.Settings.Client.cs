using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globalization;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void CreateClientSettings(PanelContent tabContent)
        {
            Tabs tabs = tabContent.Add.Tabs();
            tabs.AddTab("Language", (panelContent) =>
            {
                AddLanguageSettings(panelContent);
            }, "language");
        }

        private void AddLanguageSettings(PanelContent panelContent)
        {
            Sandbox.UI.Panel languagePanel = panelContent.Add.Panel("language");

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

            languageSelection.SelectByData(SettingsManager.Instance.General.Language);
        }
    }
}
