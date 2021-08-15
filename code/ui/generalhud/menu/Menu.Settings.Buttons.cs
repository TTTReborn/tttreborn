using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Settings;
using TTTReborn.Globals;

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
                fileSelection.DefaultSelectionPath = $"/settings/{Utils.GetTypeNameByType(SettingsManager.Instance.GetType()).ToLower()}/";
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";
                fileSelection.OnAgree = () => OnAgreeSaveAs(fileSelection);
                fileSelection.EnableFileNameEntry();
                fileSelection.Display();
            });

            buttonsWrapperPanel.Add.Button("Load from", "fileselectionbutton", () =>
            {
                FileSelection fileSelection = FindRootPanel().Add.FileSelection();
                fileSelection.DefaultSelectionPath = $"/settings/{Utils.GetTypeNameByType(SettingsManager.Instance.GetType()).ToLower()}/";
                fileSelection.DefaultSelectionFileType = $"*{SettingFunctions.SETTINGS_FILE_EXTENSION}";
                fileSelection.OnAgree = () => OnAgreeLoadFrom(fileSelection);
                fileSelection.Display();
            });
        }

        private void OnAgreeSaveAs(FileSelection fileSelection)
        {
            string fileName = fileSelection.FileNameEntry.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            fileSelection.Close();

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if (!FileSystem.Data.FileExists(fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION))
            {
                SettingFunctions.SaveSettings<ClientSettings>(SettingsManager.Instance as ClientSettings, fileSelection.CurrentFolderPath, fileName);
            }
            else
            {
                AskOverwriteSelectedSettings(fileSelection.CurrentFolderPath, fileName);
            }
        }

        private void AskOverwriteSelectedSettings(string folderPath, string fileName)
        {
            string fullFilePath = folderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION;

            DialogBox dialogBox = new DialogBox();
            dialogBox.TitleLabel.Text = $"Overwrite '{fullFilePath}'";
            dialogBox.AddText($"Do you want to overwrite '{fullFilePath}' with the current settings? (If you agree, the settings defined in this file will be lost!)");
            dialogBox.OnAgree = () =>
            {
                SettingFunctions.SaveSettings<ClientSettings>(SettingsManager.Instance as ClientSettings, folderPath, fileName);

                dialogBox.Close();
            };
            dialogBox.OnDecline = () =>
            {
                dialogBox.Close();
            };

            FindRootPanel().AddChild(dialogBox);

            dialogBox.Display();
        }

        private void OnAgreeLoadFrom(FileSelection fileSelection)
        {
            string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            fileName = fileName.Split('/')[^1].Split('.')[0];

            SettingsManager.Instance = SettingFunctions.LoadSettings<ClientSettings>(fileSelection.CurrentFolderPath, fileName);

            if (SettingsManager.Instance.LoadingError != SettingsLoadingError.None)
            {
                Log.Error($"Settings file '{fileSelection.CurrentFolderPath}{fileName}{SettingFunctions.SETTINGS_FILE_EXTENSION}' can't be loaded. Reason: '{SettingsManager.Instance.LoadingError.ToString()}'");

                return;
            }

            fileSelection.Close();

            AskDefaultSettingsChange(fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION);
        }

        private void AskDefaultSettingsChange(string filePath)
        {
            DialogBox dialogBox = new DialogBox();
            dialogBox.TitleLabel.Text = "Default settings";
            dialogBox.AddText($"Do you want to use '{filePath}' as the default settings? (If you agree, the current default settings will be overwritten!)");
            dialogBox.OnAgree = () =>
            {
                SettingFunctions.SaveSettings<ClientSettings>(SettingsManager.Instance as ClientSettings);

                dialogBox.Close();
            };
            dialogBox.OnDecline = () =>
            {
                dialogBox.Close();
            };

            FindRootPanel().AddChild(dialogBox);

            dialogBox.Display();
        }
    }
}
