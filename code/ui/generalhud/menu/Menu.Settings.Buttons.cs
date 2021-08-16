using System;

using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

using TTTReborn.Globals;
using TTTReborn.Settings;

namespace TTTReborn.UI.Menu
{
    public partial class Menu
    {
        private void CreateSettingsButtons(PanelContent menuContent)
        {
            Panel buttonsWrapperPanel = menuContent.Add.Panel("wrapper");

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
                fileSelection.OnAgree = () => OnAgreeLoadFrom(fileSelection, menuContent);
                fileSelection.Display();
            });
        }

        private void OnAgreeSaveAs(FileSelection fileSelection)
        {
            string fileName = fileSelection.FileNameEntry.Text;

            if (string.IsNullOrEmpty(fileName) || SettingsTabs == null)
            {
                return;
            }

            fileSelection.Close();

            fileName = fileName.Split('/')[^1].Split('.')[0];


            if ((Utils.Realm) SettingsTabs.SelectedTab.Value == Utils.Realm.Client)
            {
                if (!FileSystem.Data.FileExists(fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION))
                {
                    SettingFunctions.SaveSettings<ClientSettings>(SettingsManager.Instance as ClientSettings, fileSelection.CurrentFolderPath, fileName);
                }
                else
                {
                    AskOverwriteSelectedSettings(fileSelection.CurrentFolderPath, fileName, () => SettingFunctions.SaveSettings<ClientSettings>(SettingsManager.Instance as ClientSettings, fileSelection.CurrentFolderPath, fileName));
                }
            }
            else
            {
                // TODO sync with server etc.
                // TODO ask for overwrite of server settings
            }
        }

        private void AskOverwriteSelectedSettings(string folderPath, string fileName, Action onConfirm)
        {
            string fullFilePath = folderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION;

            DialogBox dialogBox = new DialogBox();
            dialogBox.TitleLabel.Text = $"Overwrite '{fullFilePath}'";
            dialogBox.AddText($"Do you want to overwrite '{fullFilePath}' with the current settings? (If you agree, the settings defined in this file will be lost!)");
            dialogBox.OnAgree = () =>
            {
                onConfirm();

                dialogBox.Close();
            };
            dialogBox.OnDecline = () =>
            {
                dialogBox.Close();
            };

            FindRootPanel().AddChild(dialogBox);

            dialogBox.Display();
        }

        private void OnAgreeLoadFrom(FileSelection fileSelection, PanelContent menuContent)
        {
            string fileName = fileSelection.SelectedEntry.FileNameLabel.Text;

            if (string.IsNullOrEmpty(fileName))
            {
                return;
            }

            fileName = fileName.Split('/')[^1].Split('.')[0];

            if ((Utils.Realm) SettingsTabs.SelectedTab.Value == Utils.Realm.Client)
            {
                SettingsManager.Instance = SettingFunctions.LoadSettings<ClientSettings>(fileSelection.CurrentFolderPath, fileName);

                if (SettingsManager.Instance.LoadingError != SettingsLoadingError.None)
                {
                    Log.Error($"Settings file '{fileSelection.CurrentFolderPath}{fileName}{SettingFunctions.SETTINGS_FILE_EXTENSION}' can't be loaded. Reason: '{SettingsManager.Instance.LoadingError.ToString()}'");

                    return;
                }

                fileSelection.Close();

                // refresh settings
                menuContent.SetPanelContent(OpenSettings);

                AskDefaultSettingsChange(fileSelection.CurrentFolderPath + fileName + SettingFunctions.SETTINGS_FILE_EXTENSION, () => SettingFunctions.SaveSettings<ClientSettings>(SettingsManager.Instance as ClientSettings));
            }
            else
            {
                // TODO sync with server etc.
                // TODO ask for overwrite of server settings
            }
        }

        private void AskDefaultSettingsChange(string filePath, Action onConfirm)
        {
            DialogBox dialogBox = new DialogBox();
            dialogBox.TitleLabel.Text = "Default settings";
            dialogBox.AddText($"Do you want to use '{filePath}' as the default settings? (If you agree, the current default settings will be overwritten!)");
            dialogBox.OnAgree = () =>
            {
                onConfirm();

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
