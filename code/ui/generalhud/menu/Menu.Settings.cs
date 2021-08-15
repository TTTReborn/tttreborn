using Sandbox.UI;
using Sandbox.UI.Construct;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void OpenSettings(PanelContent menuContent)
        {
            menuContent.SetPanelContent((panelContent) =>
            {
                CreateSettingsButtons(panelContent);
            }, "Settings", "settings");
        }

        private void CreateSettingsButtons(PanelContent panelContent)
        {
            Panel buttonsWrapperPanel = panelContent.Add.Panel("wrapper");

            buttonsWrapperPanel.Add.Button("Save as", "fileselectionbutton", () =>
            {
                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionPath = "/settings/";

                fileSelection.OnAgree = () =>
                {
                    string fileName = fileSelection.FileNameEntry.Text;

                    if (string.IsNullOrEmpty(fileName))
                    {
                        return;
                    }

                    Settings.SettingFunctions.SaveSettings(fileName.Split('/')[^1].Split('.')[0]);

                    fileSelection.Close();
                };

                fileSelection.EnableFileNameEntry();
                fileSelection.Display();
            });

            buttonsWrapperPanel.Add.Button("Load from", "fileselectionbutton", () =>
            {
                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionPath = "/settings/";

                fileSelection.OnAgree = () =>
                {
                    string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

                    if (string.IsNullOrEmpty(fileName))
                    {
                        return;
                    }

                    fileName = fileName.Split('/')[^1].Split('.')[0];

                    Settings.SettingFunctions.LoadSettings(fileName);

                    fileSelection.Close();

                    // Ask whether the player want to use the loaded settings as default ones
                    DialogBox dialogBox = new DialogBox();
                    dialogBox.TitleLabel.Text = "Default settings";
                    dialogBox.AddText($"Do you want to use '{fileName}.json' as the default settings? (If you agree, the current default settings will be overwritten!)");
                    dialogBox.OnAgree = () =>
                    {
                        Settings.SettingFunctions.SaveSettings();

                        dialogBox.Close();
                    };
                    dialogBox.OnDecline = () =>
                    {
                        dialogBox.Close();
                    };

                    FindRootPanel().AddChild(dialogBox);

                    dialogBox.Display();
                };

                fileSelection.Display();
            });
        }
    }
}
