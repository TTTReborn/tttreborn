using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void CreateSettingsButtons(PanelContent panelContent)
        {
            Panel buttonsWrapperPanel = panelContent.Add.Panel("wrapper");

            buttonsWrapperPanel.Add.Button("Save as", "fileselectionbutton", () =>
            {
                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionPath = "/settings/clientsettings/";
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";

                fileSelection.OnAgree = () =>
                {
                    string fileName = fileSelection.FileNameEntry.Text;

                    if (string.IsNullOrEmpty(fileName))
                    {
                        return;
                    }

                    fileSelection.Close();

                    fileName = fileName.Split('/')[^1].Split('.')[0];
                    string fullFilePath = fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION;

                    if (!FileSystem.Data.FileExists(fullFilePath))
                    {
                        SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance, fileSelection.CurrentFolderPath, fileName);
                    }
                    else
                    {
                        // Ask whether the player want to overwrite the selected file
                        DialogBox dialogBox = new DialogBox();
                        dialogBox.TitleLabel.Text = $"Overwrite '{fullFilePath}'";
                        dialogBox.AddText($"Do you want to overwrite '{fullFilePath}' with the current settings? (If you agree, the settings defined in this file will be lost!)");
                        dialogBox.OnAgree = () =>
                        {
                            SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance, fileSelection.CurrentFolderPath, fileName);

                            dialogBox.Close();
                        };
                        dialogBox.OnDecline = () =>
                        {
                            dialogBox.Close();
                        };

                        FindRootPanel().AddChild(dialogBox);

                        dialogBox.Display();
                    }
                };

                fileSelection.EnableFileNameEntry();
                fileSelection.Display();
            });

            buttonsWrapperPanel.Add.Button("Load from", "fileselectionbutton", () =>
            {
                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionPath = "/settings/clientsettings/";
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";

                fileSelection.OnAgree = () =>
                {
                    string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

                    if (string.IsNullOrEmpty(fileName))
                    {
                        return;
                    }

                    fileName = fileName.Split('/')[^1].Split('.')[0];

                    ClientSettings.Instance = SettingFunctions.LoadSettings<ClientSettings>(fileSelection.CurrentFolderPath, fileName);

                    if (ClientSettings.Instance.LoadingError != SettingsLoadingError.None)
                    {
                        Log.Error($"Settings file '{fileSelection.CurrentFolderPath}{fileName}{SettingFunctions.SETTINGS_FILE_EXTENSION}' can't be loaded. Reason: '{ClientSettings.Instance.LoadingError.ToString()}'");

                        return;
                    }

                    fileSelection.Close();

                    // Ask whether the player want to use the loaded settings as default ones
                    DialogBox dialogBox = new DialogBox();
                    dialogBox.TitleLabel.Text = "Default settings";
                    dialogBox.AddText($"Do you want to use '{fileSelection.CurrentFolderPath}{fileName}{SettingFunctions.SETTINGS_FILE_EXTENSION}' as the default settings? (If you agree, the current default settings will be overwritten!)");
                    dialogBox.OnAgree = () =>
                    {
                        SettingFunctions.SaveSettings<ClientSettings>(ClientSettings.Instance);

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
